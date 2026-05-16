using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VenusHR.Application.Common.Interfaces.HR_Master;
using VenusHR.Application.Common.Interfaces.Login;
using VenusHR.Core.Login;
using VenusHR.Infrastructure.Presistence;
using VenusHR.API.Models;
using WorkFlow_EF;

namespace VenusHR.API.Controllers.Login
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private ApplicationDBContext _context;
        private readonly ILoginServices _Login;
        private readonly IHRMaster _HRMaster;


        public LoginController(ILoginServices login, ApplicationDBContext context)
        {
            _context = context;
            _Login = login;
        }
        [HttpPost, Route("Login")]
        public ActionResult<ApiResponse<object>> Login([FromBody] SysUser User, [FromQuery] int Lang)
        {
            try
            {
                var Result = _Login.Login(User.Code, User.Password, Lang, User.DeviceToken);

                if (Result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل تسجيل الدخول" : "Login failed";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم تسجيل الدخول بنجاح" : "Login successful";
                    return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                }

                var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }
    }
}
