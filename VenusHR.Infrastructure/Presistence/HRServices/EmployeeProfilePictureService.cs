using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VenusHR.Application.Common.Interfaces.HR_Master;
using VenusHR.Application.Common.Interfaces.SelfService;
using VenusHR.Core.Master;
using WorkFlow_EF;

namespace VenusHR.Infrastructure.Presistence.HRServices
{
    public class EmployeeProfilePictureService : IEmployeeProfilePicture
    {
        private const string EmployeesObjectCode = "hrs_Employees";
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

        private readonly ApplicationDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMaster _master;

        public EmployeeProfilePictureService(ApplicationDBContext context, IConfiguration configuration, IMaster master)
        {
            _context = context;
            _configuration = configuration;
            _master = master;
        }

        public async Task<GeneralOutputClass<object>> UploadProfilePictureAsync(int employeeId, Stream fileStream, string fileName, int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            string? savedFilePath = null;
            try
            {
                var employee = await _context.Hrs_Employees
                    .Where(E => E.id == employeeId)
                    .Select(E => new { E.id, E.CompanyId })
                    .FirstOrDefaultAsync();
                if (employee == null)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = lang == 1 ? "الموظف غير موجود" : "Employee not found";
                    return result;
                }

                var canUpload = await _context.sys_MobileConfig
                    .AnyAsync(C => C.CompanyId == employee.CompanyId && C.CancelDate == null && C.EmpCanUploadProfilePic);
                if (!canUpload)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = lang == 1
                        ? "غير مسموح للموظفين برفع الصورة الشخصية"
                        : "Employees are not allowed to upload a profile picture";
                    return result;
                }

                var extension = Path.GetExtension(fileName ?? string.Empty).ToLowerInvariant();
                if (!AllowedExtensions.Contains(extension))
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = lang == 1
                        ? $"امتدادات الصور المسموحة: {string.Join(", ", AllowedExtensions)}"
                        : $"Allowed photo extensions: {string.Join(", ", AllowedExtensions)}";
                    return result;
                }

                if (fileStream.CanSeek && fileStream.Length > MaxFileSizeBytes)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = lang == 1 ? "حجم الصورة يتجاوز 5 ميجابايت" : "Photo size exceeds 5 MB";
                    return result;
                }

                var objectId = Convert.ToInt32(_master.GetSysObjectId(EmployeesObjectCode));
                if (objectId <= 0)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = $"sys_Objects entry for '{EmployeesObjectCode}' not found";
                    return result;
                }

                var baseName = SanitizeFileName(Path.GetFileNameWithoutExtension(fileName));
                var storedFileName = $"{baseName}_{DateTime.Now:ddMMyyyyHHmmsss}{extension}";
                var folderPath = Path.Combine(GetPhotosRootPath(), $"{objectId}_{employeeId}");
                Directory.CreateDirectory(folderPath);
                savedFilePath = Path.Combine(folderPath, storedFileName);

                using (var output = new FileStream(savedFilePath, FileMode.Create))
                {
                    await fileStream.CopyToAsync(output);
                }

                var previousDefaults = await _context.sys_ObjectsAttachments
                    .Where(A => A.ObjectID == objectId && A.RecordID == employeeId && A.IsProfilePicture == true)
                    .ToListAsync();
                foreach (var previous in previousDefaults)
                {
                    previous.IsProfilePicture = false;
                }

                var attachment = new sys_ObjectsAttachments
                {
                    ObjectID = objectId,
                    RecordID = employeeId,
                    FileName = storedFileName,
                    IsImageView = true,
                    IsProfilePicture = true,
                    IsDGSignature = false,
                    RegDate = DateTime.Now
                };
                _context.sys_ObjectsAttachments.Add(attachment);
                await _context.SaveChangesAsync();

                result.ErrorCode = 1;
                result.ResultObject = new
                {
                    AttachmentId = attachment.ID,
                    EmployeeId = employeeId,
                    FileName = storedFileName,
                    ContentType = GetContentType(extension),
                    attachment.RegDate
                };
            }
            catch (Exception ex)
            {
                if (savedFilePath != null && File.Exists(savedFilePath))
                {
                    File.Delete(savedFilePath);
                }
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> GetProfilePictureAsync(int employeeId, int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                var objectId = Convert.ToInt32(_master.GetSysObjectId(EmployeesObjectCode));
                var now = DateTime.Now;

                var attachment = await _context.sys_ObjectsAttachments
                    .Where(A => A.ObjectID == objectId
                             && A.RecordID == employeeId
                             && A.IsProfilePicture == true
                             && (A.IsDGSignature == null || A.IsDGSignature == false)
                             && A.IsImageView != null
                             && A.CancelDate == null
                             && (A.ExpiryDate == null || A.ExpiryDate > now))
                    .OrderByDescending(A => A.ID)
                    .FirstOrDefaultAsync();

                var notFoundMessage = lang == 1 ? "لا توجد صورة شخصية" : "Profile picture not found";
                if (attachment == null || string.IsNullOrEmpty(attachment.FileName))
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = notFoundMessage;
                    return result;
                }

                var fullPath = Path.Combine(GetPhotosRootPath(), $"{objectId}_{employeeId}", attachment.FileName);
                if (!File.Exists(fullPath))
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = notFoundMessage;
                    return result;
                }

                result.ErrorCode = 1;
                result.ResultObject = new EmployeeProfilePictureFile(
                    fullPath,
                    attachment.FileName,
                    GetContentType(Path.GetExtension(attachment.FileName).ToLowerInvariant()));
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        // Must point at the legacy web site's "Photos" folder so both apps share the same files.
        private string GetPhotosRootPath()
        {
            var configured = _configuration["FileUpload:EmployeesPhotosPath"];
            if (!string.IsNullOrWhiteSpace(configured))
                return configured;

            var uploadsPath = _configuration["FileUpload:UploadsPath"] ?? "D:\\Uploads";
            return Path.Combine(uploadsPath, "Photos");
        }

        private static string SanitizeFileName(string? name)
        {
            name ??= string.Empty;
            var invalid = Path.GetInvalidFileNameChars();
            var cleaned = new string(name.Where(c => !invalid.Contains(c)).ToArray()).Trim();
            return string.IsNullOrEmpty(cleaned) ? "photo" : cleaned;
        }

        private static string GetContentType(string extension) => extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };
    }
}
