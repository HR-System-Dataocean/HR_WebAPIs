using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VenusHR.Application.Common.Interfaces.HR_Master;
using VenusHR.Application.Common.Interfaces.Login;
using VenusHR.Core.Login;
using VenusHR.Infrastructure.Presistence;

namespace VenusHR.API.Controllers.Login
{
    [Route("api/[controller]")]
    [ApiController]
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
        public ActionResult<ILoginServices> Login([FromBody] SysUser User, [FromQuery] int Lang)
        {

            var Result = _Login.Login(User.Code, User.Password, Lang, User.DeviceToken);
            

            return Ok(Result);
        }
    }
}
