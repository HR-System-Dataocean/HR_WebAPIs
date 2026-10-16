using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VenusHR.API.Models;
using VenusHR.Application.Common.Interfaces.HR_Master;

namespace VenusHR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeProfilePictureController : ControllerBase
    {
        private readonly IEmployeeProfilePicture _profilePicture;

        public EmployeeProfilePictureController(IEmployeeProfilePicture profilePicture)
        {
            _profilePicture = profilePicture;
        }

        [HttpPost("Upload")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<ActionResult<ApiResponse<object>>> Upload([FromForm] UploadProfilePictureRequest request, [FromQuery] int Lang = 0)
        {
            try
            {
                var employeeId = GetCurrentEmployeeId();
                if (employeeId <= 0)
                {
                    return Unauthorized(ApiResponse<object>.Fail(Lang == 1 ? "المستخدم غير معروف" : "Unknown user"));
                }

                if (request?.File == null || request.File.Length == 0)
                {
                    return BadRequest(ApiResponse<object>.Fail(Lang == 1 ? "الصورة مطلوبة" : "Photo is required"));
                }

                using var stream = request.File.OpenReadStream();
                var result = await _profilePicture.UploadProfilePictureAsync(employeeId, stream, request.File.FileName, Lang);

                if (result.ErrorCode == 0)
                {
                    var message = Lang == 1 ? "فشل رفع الصورة" : "Failed to upload photo";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = Lang == 1 ? "تم رفع الصورة بنجاح" : "Photo uploaded successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, successMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("GetMyProfilePicture")]
        public Task<ActionResult> GetMyProfilePicture([FromQuery] int Lang = 0)
        {
            return GetProfilePicture(GetCurrentEmployeeId(), Lang);
        }

        [HttpGet("GetProfilePicture/{employeeId}")]
        public async Task<ActionResult> GetProfilePicture(int employeeId, [FromQuery] int Lang = 0)
        {
            try
            {
                var result = await _profilePicture.GetProfilePictureAsync(employeeId, Lang);

                if (result.ErrorCode == 0 || result.ResultObject is not EmployeeProfilePictureFile picture)
                {
                    var message = Lang == 1 ? "لا توجد صورة شخصية" : "Profile picture not found";
                    return NotFound(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                return PhysicalFile(picture.FullPath, picture.ContentType);
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        private int GetCurrentEmployeeId()
        {
            var claim = User.FindFirst("EmployeeId")?.Value;
            return int.TryParse(claim, out var id) ? id : 0;
        }
    }

    public class UploadProfilePictureRequest
    {
        public IFormFile File { get; set; } = null!;
    }
}
