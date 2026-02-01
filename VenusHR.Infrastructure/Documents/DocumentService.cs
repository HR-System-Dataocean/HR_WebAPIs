using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using VenusHR.Application.Common.Interfaces.Documents;
using VenusHR.Core.Master;
using VenusHR.Infrastructure.Presistence;
using WorkFlow_EF;

namespace VenusHR.Infrastructure.Services.Documents
{
    public class DocumentsService : IDocumentsService
    {
        private readonly ApplicationDBContext _context;
        private readonly IConfiguration _configuration;
        private GeneralOutputClass<object> _result;

        public DocumentsService(ApplicationDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _result = new GeneralOutputClass<object>();
        }

        public object SaveAttachment(int documentId, int objectId, long recordId, Stream fileStream,
                                    string fileName, string engName, string arbName,
                                    DateTime? issueDate = null, int? issuedCityId = null,
                                    DateTime? expiryDate = null, string? documentNumber = null,
                                    string? referenceNumber = null, DateTime? lastRenewalDate = null,
                                    string? folderName = null)
        {
            try
            {
                //// 1. التحقق من وجود نوع المستند
                //var documentType = _context.sys_Documents
                //    .FirstOrDefault(d => d.ID == documentId);

                //if (documentType == null)
                //{
                //    _result.ErrorCode = 0;
                //    _result.ErrorMessage = "نوع المستند غير موجود";
                //    return _result;
                //}

                // 2. إنشاء DocumentDetail أولاً
                var documentDetail = new sys_DocumentsDetails
                {
                    DocumentID = documentId,
                    ObjectID = objectId,
                    RecordID = (int)recordId, // تحويل من long لـ int
                    DocumentNumber = documentNumber ?? GenerateDocumentNumber(),
                    IssueDate = issueDate,
                    IssuedCityID = issuedCityId,
                    ExpiryDate = expiryDate,
                    LastRenewalDate = lastRenewalDate,
                    ReferenceNumber = referenceNumber,
                    RegUserID = 1, // TODO: Get from current user
                    RegComputerID = 1, // TODO: Get computer ID
                    RegDate = DateTime.Now
                };

                _context.sys_DocumentsDetails.Add(documentDetail);
                _context.SaveChanges(); // Save أولاً علشان نجيب الـ ID

                // 3. قراءة المسار من الـ Configuration
                var uploadsBasePath = _configuration["FileUpload:UploadsPath"] ?? "D:\\Uploads\\VenusHR";
                var baseUrl = _configuration["FileUpload:BaseUrl"] ?? "/uploads";

                // إذا تم تحديد folderName، أضفه للمسار
                if (!string.IsNullOrEmpty(folderName))
                {
                    uploadsBasePath = Path.Combine(uploadsBasePath, folderName);
                    baseUrl = $"{baseUrl.TrimEnd('/')}/{folderName}";
                }

                // التحقق من صحة الامتداد
                var fileExtension = Path.GetExtension(fileName).ToLower();
                string[] allowedExtensions = {
                    ".pdf", ".doc", ".docx", ".xls", ".xlsx",
                    ".jpg", ".jpeg", ".png", ".tiff", ".ppt",
                    ".mp3", ".wav", ".aac", ".ogg", ".flac", ".m4a", ".wma",
                    ".pptx", ".txt", ".rtf"
                };

                if (!allowedExtensions.Contains(fileExtension))
                {
                    // إذا فشل التحقق، نحذف الـ DocumentDetail اللي حفظناه
                    _context.sys_DocumentsDetails.Remove(documentDetail);
                    _context.SaveChanges();

                    _result.ErrorCode = 0;
                    _result.ErrorMessage = "امتداد الملف غير مسموح به";
                    return _result;
                }

                // إنشاء المجلد إذا لم يكن موجوداً
                if (!Directory.Exists(uploadsBasePath))
                {
                    Directory.CreateDirectory(uploadsBasePath);
                }

                // إنشاء اسم فريد للملف
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                var uniqueFileName = $"{fileNameWithoutExt}_{DateTime.Now:ddMMyyyyHHmmsss}{fileExtension}";
                var filePath = Path.Combine(uploadsBasePath, uniqueFileName);

                // حفظ الملف على السيرفر
                using (var fileStream2 = new FileStream(filePath, FileMode.Create))
                {
                    fileStream.CopyTo(fileStream2);
                }

                // الحصول على حجم الملف
                var fileInfo = new FileInfo(filePath);

                // حفظ المعلومات في قاعدة البيانات
                // استخراج اسم المجلد الأخير فقط للحفظ في قاعدة البيانات
                var folderForDb = !string.IsNullOrEmpty(folderName) ? folderName : new DirectoryInfo(uploadsBasePath).Name;

                var attachment = new sys_ObjectsAttachments
                {
                    ObjectID = objectId,
                    RecordID = documentDetail.ID, // RecordID هنا بيكون الـ ID الخاص بالـ DocumentDetail
                    EngName = engName,
                    ArbName = arbName,
                    ArbName4S = arbName,
                    FileName = uniqueFileName,
                    FolderName = folderForDb,
                    RegUserID = 1, // TODO: Get from current user
                    RegComputerID = 1, // TODO: Get computer ID
                    RegDate = DateTime.Now
                };

                _context.sys_ObjectsAttachments.Add(attachment);
                _context.SaveChanges(); // Save تاني للـ Attachment

                // إنشاء المسار النسبي للـ URL
                var relativePath = $"{baseUrl.TrimEnd('/')}/{uniqueFileName}";

                _result.ErrorCode = 1;
                _result.ErrorMessage = "تم حفظ المرفق والمستند بنجاح";
                _result.ResultObject = new
                {
                    Success = true,
                    Message = "تم حفظ المرفق والمستند بنجاح",
                    DocumentDetailId = documentDetail.ID,
                    AttachmentId = attachment.ID,
                    DocumentNumber = documentDetail.DocumentNumber,
                    FileName = uniqueFileName,
                    FilePath = relativePath,
                    FileSize = fileInfo.Length,
                    ContentType = GetContentType(fileExtension),
                    RegDate = attachment.RegDate
                };

                return _result;
            }
            catch (Exception ex)
            {
                _result.ErrorCode = 0;
                _result.ErrorMessage = $"حدث خطأ أثناء حفظ المرفق: {ex.Message}";
                return _result;
            }
        }
        public object GetAttachments(int objectId, long recordId)
        {
            try
            {
                var baseUrl = _configuration["FileUpload:BaseUrl"] ?? "/uploads";

                var attachments = _context.sys_DocumentsDetails
    .Where(d => d.RecordID == recordId && d.CancelDate == null && d.ObjectID == objectId)
    .Join(
        _context.sys_ObjectsAttachments.Where(a => a.CancelDate == null),
        d => new {
            RecordID = (long)d.ID,
            ObjectID = (long)d.ObjectID
        },
        a => new {
            RecordID = a.RecordID,
            ObjectID = (long)a.ObjectID
        },
        (documentDetail, attachment) => new
        {
            ID = (long)attachment.ID,
            attachment.FileName,
            attachment.EngName,
            attachment.ArbName,
            FilePath = $"{baseUrl.TrimEnd('/')}/{attachment.FolderName}/{attachment.FileName}",
            attachment.RegDate,
            AttachmentExpiryDate = attachment.ExpiryDate,
            DocumentDetailId = (long)documentDetail.ID,
            DocumentNumber = documentDetail.DocumentNumber,
            DocumentId = (long?)documentDetail.DocumentID,
            IssueDate = documentDetail.IssueDate,
            DocumentExpiryDate = documentDetail.ExpiryDate,
            ReferenceNumber = documentDetail.ReferenceNumber,
            IssuedCityID = documentDetail.IssuedCityID,
            LastRenewalDate = documentDetail.LastRenewalDate
        }
    )
    .ToList();


                _result.ErrorCode = 1;
                _result.ErrorMessage = "Success";
                _result.ResultObject = attachments;
                return _result;
            }
            catch (Exception ex)
            {
                _result.ErrorCode = 0;
                _result.ErrorMessage = $"حدث خطأ أثناء جلب المرفقات: {ex.Message}";
                return _result;
            }
        }
        public object DeleteAttachment(int attachmentId)
        {
            try
            {
                var attachment = _context.sys_ObjectsAttachments
                    .FirstOrDefault(a => a.ID == attachmentId && a.CancelDate == null);

                if (attachment == null)
                {
                    _result.ErrorCode = 0;
                    _result.ErrorMessage = "المرفق غير موجود";
                    return _result;
                }

                // حذف الـ DocumentDetail المرتبط أولاً
                var documentDetail = _context.sys_DocumentsDetails
                    .FirstOrDefault(d => d.ID == attachment.RecordID && d.CancelDate == null);

                if (documentDetail != null)
                {
                    documentDetail.CancelDate = DateTime.Now;
                }

                // حذف الملف من السيرفر
                if (!string.IsNullOrEmpty(attachment.FileName) && !string.IsNullOrEmpty(attachment.FolderName))
                {
                    var uploadsBasePath = _configuration["FileUpload:UploadsPath"] ?? "D:\\Uploads\\VenusHR";
                    var fullPath = Path.Combine(uploadsBasePath, attachment.FolderName, attachment.FileName);

                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }

                // تحديث حالة الـ Attachment
                attachment.CancelDate = DateTime.Now;
                _context.SaveChanges();

                _result.ErrorCode = 1;
                _result.ErrorMessage = "تم حذف المرفق والمستند بنجاح";
                return _result;
            }
            catch (Exception ex)
            {
                _result.ErrorCode = 0;
                _result.ErrorMessage = $"حدث خطأ أثناء حذف المرفق: {ex.Message}";
                return _result;
            }
        }

        public object GetAttachmentInfo(int attachmentId)
        {
            try
            {
                var baseUrl = _configuration["FileUpload:BaseUrl"] ?? "/uploads";
                var uploadsBasePath = _configuration["FileUpload:UploadsPath"] ?? "D:\\Uploads\\VenusHR";
                var allAttachments = _context.sys_ObjectsAttachments
          .Where(a => a.CancelDate == null)
          .ToList();

                var attachment = allAttachments
                    .FirstOrDefault(a => a.ID == attachmentId);
                if (attachment == null)
                {
                    _result.ErrorCode = 0;
                    _result.ErrorMessage = "المرفق غير موجود";
                    return _result;
                }

                long fileSize = 0;
                string contentType = "";

                if (!string.IsNullOrEmpty(attachment.FileName) && !string.IsNullOrEmpty(attachment.FolderName))
                {
                    //var fullPath = Path.Combine(uploadsBasePath, attachment.FolderName, attachment.FileName);
                    var fullPath = Path.Combine(uploadsBasePath,  attachment.FileName);

                    if (File.Exists(fullPath))
                    {
                        var fileInfo = new FileInfo(fullPath);
                        fileSize = fileInfo.Length;
                        contentType = GetContentType(Path.GetExtension(attachment.FileName).ToLower());
                    }
                }

                // جلب بيانات الـ DocumentDetail المرتبط
                var documentDetail = _context.sys_DocumentsDetails
                    .FirstOrDefault(d => d.ID == attachment.RecordID && d.CancelDate == null);

                _result.ErrorCode = 1;
                _result.ErrorMessage = "Success";
                _result.ResultObject = new
                {
                    attachment.ID,
                    attachment.FileName,
                    attachment.EngName,
                    attachment.ArbName,
                    FilePath = $"{baseUrl.TrimEnd('/')}/{attachment.FileName}",
                    FullPath = Path.Combine(uploadsBasePath, attachment.FileName),
                    FileSize = fileSize,
                    attachment.RegDate,
                    attachment.ExpiryDate,
                    ContentType = contentType,
                    DocumentDetail = documentDetail != null ? new
                    {
                        documentDetail.ID,
                        documentDetail.DocumentNumber,
                        documentDetail.IssueDate,
                        documentDetail.ExpiryDate,
                        documentDetail.LastRenewalDate,
                        documentDetail.ReferenceNumber,
                        documentDetail.IssuedCityID,
                        documentDetail.DocumentID
                    } : null
                };
                return _result;
            }
            catch (Exception ex)
            {
                _result.ErrorCode = 0;
                _result.ErrorMessage = $"حدث خطأ أثناء جلب معلومات المرفق: {ex.Message}";
                return _result;
            }
        }

        public object AddDocumentDetail(int documentId, int objectId, int recordId,
                                        string documentNumber, DateTime? issueDate,
                                        int? issuedCityId, DateTime? expiryDate,
                                        string? referenceNumber = null,
                                        DateTime? lastRenewalDate = null)
        {
            try
            {
                // التحقق من وجود المستند
                var document = _context.sys_Documents
                    .FirstOrDefault(d => d.ID == documentId);

                if (document == null)
                {
                    _result.ErrorCode = 0;
                    _result.ErrorMessage = "نوع المستند غير موجود";
                    return _result;
                }

                // التحقق من عدم تكرار رقم المستند
                var existing = _context.sys_DocumentsDetails
                    .FirstOrDefault(d => d.DocumentNumber == documentNumber &&
                                         d.CancelDate == null);

                if (existing != null)
                {
                    _result.ErrorCode = 0;
                    _result.ErrorMessage = "رقم المستند موجود مسبقاً";
                    return _result;
                }

                // إنشاء المستند الجديد
                var documentDetail = new sys_DocumentsDetails
                {
                    DocumentID = documentId,
                    ObjectID = objectId,
                    RecordID = recordId,
                    DocumentNumber = documentNumber,
                    IssueDate = issueDate,
                    IssuedCityID = issuedCityId,
                    ExpiryDate = expiryDate,
                    LastRenewalDate = lastRenewalDate,
                    ReferenceNumber = referenceNumber,
                    RegUserID = 1, // TODO: Get from current user
                    RegComputerID = 1, // TODO: Get computer ID
                    RegDate = DateTime.Now
                };

                _context.sys_DocumentsDetails.Add(documentDetail);
                _context.SaveChanges();

                _result.ErrorCode = 1;
                _result.ErrorMessage = "تم إضافة المستند بنجاح";
                _result.ResultObject = new { DocumentDetailId = documentDetail.ID };
                return _result;
            }
            catch (Exception ex)
            {
                _result.ErrorCode = 0;
                _result.ErrorMessage = $"حدث خطأ أثناء إضافة المستند: {ex.Message}";
                return _result;
            }
        }

        public object GetDocumentDetails(int objectId, int recordId)
        {
            try
            {
                var documentDetails = _context.sys_DocumentsDetails
                    .Where(d => d.ObjectID == objectId &&
                                d.RecordID == recordId &&
                                d.CancelDate == null)
                    .Join(_context.sys_Documents,
                        detail => detail.DocumentID,
                        doc => doc.ID,
                        (detail, doc) => new
                        {
                            detail.ID,
                            detail.DocumentNumber,
                            detail.IssueDate,
                            detail.ExpiryDate,
                            detail.LastRenewalDate,
                            detail.ReferenceNumber,
                            detail.IssuedCityID,
                            detail.Remarks,
                            DocumentName = doc.EngName,
                            DocumentArabicName = doc.ArbName,
                            DocumentId = doc.ID,
                            // جلب الـ Attachment إذا وجد
                            Attachment = _context.sys_ObjectsAttachments
                                .Where(a => a.RecordID == detail.ID && a.CancelDate == null)
                                .Select(a => new
                                {
                                    a.ID,
                                    a.FileName,
                                    a.EngName,
                                    a.ArbName
                                })
                                .FirstOrDefault()
                        })
                    .ToList();

                _result.ErrorCode = 1;
                _result.ErrorMessage = "Success";
                _result.ResultObject = documentDetails;
                return _result;
            }
            catch (Exception ex)
            {
                _result.ErrorCode = 0;
                _result.ErrorMessage = $"حدث خطأ أثناء جلب المستندات: {ex.Message}";
                return _result;
            }
        }

        public object GetDocumentTypes(bool? isForCompany = null, int? documentTypesGroupId = null)
        {
            try
            {
                var query = _context.sys_Documents.Where(d => d.CancelDate == null);

                if (isForCompany.HasValue)
                {
                    query = query.Where(d => d.IsForCompany == isForCompany.Value);
                }

                if (documentTypesGroupId.HasValue)
                {
                    query = query.Where(d => d.DocumentTypesGroupId == documentTypesGroupId.Value);
                }

                var documents = query.Select(d => new
                {
                    d.ID,
                    d.Code,
                    d.EngName,
                    d.ArbName,
                    d.IsForCompany,
                    d.DocumentTypesGroupId
                }).ToList();

                _result.ErrorCode = 1;
                _result.ErrorMessage = "Success";
                _result.ResultObject = documents;
                return _result;
            }
            catch (Exception ex)
            {
                _result.ErrorCode = 0;
                _result.ErrorMessage = $"حدث خطأ أثناء جلب أنواع المستندات: {ex.Message}";
                return _result;
            }
        }

        public object GetDocumentDetail(int documentDetailId)
        {
            try
            {
                var documentDetail = _context.sys_DocumentsDetails
                    .Where(d => d.ID == documentDetailId && d.CancelDate == null)
                    .Join(_context.sys_Documents,
                        detail => detail.DocumentID,
                        doc => doc.ID,
                        (detail, doc) => new
                        {
                            detail.ID,
                            detail.DocumentNumber,
                            detail.IssueDate,
                            detail.ExpiryDate,
                            detail.LastRenewalDate,
                            detail.ReferenceNumber,
                            detail.IssuedCityID,
                            detail.Remarks,
                            DocumentName = doc.EngName,
                            DocumentArabicName = doc.ArbName,
                            DocumentId = doc.ID
                        })
                    .FirstOrDefault();

                if (documentDetail == null)
                {
                    _result.ErrorCode = 0;
                    _result.ErrorMessage = "المستند غير موجود";
                    return _result;
                }

                _result.ErrorCode = 1;
                _result.ErrorMessage = "Success";
                _result.ResultObject = documentDetail;
                return _result;
            }
            catch (Exception ex)
            {
                _result.ErrorCode = 0;
                _result.ErrorMessage = $"حدث خطأ أثناء جلب تفاصيل المستند: {ex.Message}";
                return _result;
            }
        }

        public object UpdateDocumentDetail(int documentDetailId, string? documentNumber = null,
                                          DateTime? issueDate = null, int? issuedCityId = null,
                                          DateTime? expiryDate = null, DateTime? lastRenewalDate = null,
                                          string? referenceNumber = null)
        {
            try
            {
                var documentDetail = _context.sys_DocumentsDetails
                    .FirstOrDefault(d => d.ID == documentDetailId && d.CancelDate == null);

                if (documentDetail == null)
                {
                    _result.ErrorCode = 0;
                    _result.ErrorMessage = "المستند غير موجود";
                    return _result;
                }

                // التحقق من عدم تكرار رقم المستند إذا تم التعديل
                if (!string.IsNullOrEmpty(documentNumber) && documentNumber != documentDetail.DocumentNumber)
                {
                    var existing = _context.sys_DocumentsDetails
                        .FirstOrDefault(d => d.DocumentNumber == documentNumber &&
                                             d.ID != documentDetailId &&
                                             d.CancelDate == null);

                    if (existing != null)
                    {
                        _result.ErrorCode = 0;
                        _result.ErrorMessage = "رقم المستند موجود مسبقاً";
                        return _result;
                    }
                }

                // التحديث
                if (!string.IsNullOrEmpty(documentNumber))
                    documentDetail.DocumentNumber = documentNumber;

                if (issueDate.HasValue)
                    documentDetail.IssueDate = issueDate;

                if (issuedCityId.HasValue)
                    documentDetail.IssuedCityID = issuedCityId;

                if (expiryDate.HasValue)
                    documentDetail.ExpiryDate = expiryDate;

                if (lastRenewalDate.HasValue)
                    documentDetail.LastRenewalDate = lastRenewalDate;

                if (!string.IsNullOrEmpty(referenceNumber))
                    documentDetail.ReferenceNumber = referenceNumber;

                _context.SaveChanges();

                _result.ErrorCode = 1;
                _result.ErrorMessage = "تم تحديث المستند بنجاح";
                return _result;
            }
            catch (Exception ex)
            {
                _result.ErrorCode = 0;
                _result.ErrorMessage = $"حدث خطأ أثناء تحديث المستند: {ex.Message}";
                return _result;
            }
        }

        public object DeleteDocumentDetail(int documentDetailId)
        {
            try
            {
                var documentDetail = _context.sys_DocumentsDetails
                    .FirstOrDefault(d => d.ID == documentDetailId && d.CancelDate == null);

                if (documentDetail == null)
                {
                    _result.ErrorCode = 0;
                    _result.ErrorMessage = "المستند غير موجود";
                    return _result;
                }

                // حذف الـ Attachment المرتبط إذا وجد
                var attachment = _context.sys_ObjectsAttachments
                    .FirstOrDefault(a => a.RecordID == documentDetailId && a.CancelDate == null);

                if (attachment != null)
                {
                    // حذف الملف من السيرفر
                    if (!string.IsNullOrEmpty(attachment.FileName) && !string.IsNullOrEmpty(attachment.FolderName))
                    {
                        var uploadsBasePath = _configuration["FileUpload:UploadsPath"] ?? "D:\\Uploads\\VenusHR";
                        var fullPath = Path.Combine(uploadsBasePath, attachment.FolderName, attachment.FileName);

                        if (File.Exists(fullPath))
                        {
                            File.Delete(fullPath);
                        }
                    }

                    attachment.CancelDate = DateTime.Now;
                }

                documentDetail.CancelDate = DateTime.Now;
                _context.SaveChanges();

                _result.ErrorCode = 1;
                _result.ErrorMessage = "تم حذف المستند بنجاح";
                return _result;
            }
            catch (Exception ex)
            {
                _result.ErrorCode = 0;
                _result.ErrorMessage = $"حدث خطأ أثناء حذف المستند: {ex.Message}";
                return _result;
            }
        }

        private string GenerateDocumentNumber()
        {
            var lastDocument = _context.sys_DocumentsDetails
                .OrderByDescending(d => d.ID)
                .FirstOrDefault();

            if (lastDocument != null && !string.IsNullOrEmpty(lastDocument.DocumentNumber))
            {
                if (int.TryParse(lastDocument.DocumentNumber, out int lastNumber))
                {
                    return (lastNumber + 1).ToString().PadLeft(6, '0');
                }
            }

            return "000001";
        }

        private string GetContentType(string fileExtension)
        {
            return fileExtension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".tiff" => "image/tiff",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".txt" => "text/plain",
                ".rtf" => "application/rtf",
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".aac" => "audio/aac",
                ".ogg" => "audio/ogg",
                ".flac" => "audio/flac",
                ".m4a" => "audio/mp4",
                ".wma" => "audio/x-ms-wma",
                _ => "application/octet-stream"
            };
        }
    }
}