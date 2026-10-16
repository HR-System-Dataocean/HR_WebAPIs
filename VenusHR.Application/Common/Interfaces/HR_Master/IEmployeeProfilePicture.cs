using System.IO;
using System.Threading.Tasks;
using WorkFlow_EF;

namespace VenusHR.Application.Common.Interfaces.HR_Master
{
    public interface IEmployeeProfilePicture
    {
        Task<GeneralOutputClass<object>> UploadProfilePictureAsync(int employeeId, Stream fileStream, string fileName, int lang = 0);

        /// <summary>ResultObject is an <see cref="EmployeeProfilePictureFile"/> when found.</summary>
        Task<GeneralOutputClass<object>> GetProfilePictureAsync(int employeeId, int lang = 0);
    }

    public record EmployeeProfilePictureFile(string FullPath, string FileName, string ContentType);
}
