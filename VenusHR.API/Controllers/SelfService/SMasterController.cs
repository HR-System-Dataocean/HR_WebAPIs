using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VenusHR.Application.Common.Interfaces.SelfService;
using VenusHR.Core.Master;
using VenusHR.Core.SelfService;
using VenusHR.Infrastructure.Presistence;
using VenusHR.Infrastructure.Presistence.SelfService;

namespace VenusHR.API.Controllers.SelfService
{
    [Route("api/[controller]")]
    [ApiController]
    public class SMasterController : ControllerBase
    {
        private ApplicationDBContext _context;
        private readonly IMaster _Master;
        public SMasterController(IMaster Master, ApplicationDBContext context)
        {
            _context = context;
            _Master = Master;



        }

        [HttpGet, Route("GetAllRequestTypes")]
        public ActionResult<IMaster> GetAllRequestTypes()
        {
           
            var Result = _Master.GetAllRequestTypes();
            return Ok(Result);
        }
        [HttpGet, Route("GetAllEmployees/{Lang}")]
        public ActionResult<IMaster> GetAllEmployees(int Lang)
        {
            var Result = _Master.GetAllEmployees(Lang);
            return Ok(Result);
        }
        [HttpGet, Route("GetEmployeeByID/{id}")]
        public ActionResult<Hrs_Employees> GetEmployeeByID(int id,int Lang)
        {
            object Result = _Master.GetEmployeeByID(id,Lang);
            return Ok(Result);
        }

        [HttpGet, Route("GetUserNotificationCount/{EmployeeID}")]
        public ActionResult<SS_RequestAction> GetUserNotificationCount(string EmployeeID)
        {
            object Result = _Master.GetUserNotificationCount(EmployeeID);
            return Ok(Result);
        }
        [HttpGet, Route("GetAllVacationsTypes/{Lang}")]
        public ActionResult  GetAllVacationsTypes(int Lang) 
        {
            object Result = _Master.GetAllVacationsTypes(Lang);
            return Ok(Result);
        }

        [HttpGet, Route("GetEndOfServiceAllResignationReason/{Lang}")]
        public ActionResult GetEndOfServiceAllResignationReason(int Lang)
        {
            object Result = _Master.GetEndOfServiceAllResignationReason(Lang);
            return Ok(Result);
        }

        [HttpGet, Route("GetEndOfServiceAllExperienceRate/{Lang}")]
        public ActionResult GetEndOfServiceAllExperienceRate(int Lang)
        {
            object Result = _Master.GetEndOfServiceAllExperienceRate(Lang);
            return Ok(Result);
        }
        [HttpGet, Route("GetAllPendingRequests/{EmployeeID}/{Lang}")]
        public ActionResult<SS_RequestAction> GetAllPendingRequests(int EmployeeID,int Lang)
        {
            object Result = _Master.GetAllPendingRequests(EmployeeID, Lang);
            return Ok(Result);
        }

        [HttpPost, Route("SaveRequestAction")]
        public ActionResult<SS_RequestAction> SaveRequestAction(SS_RequestAction RequestAction)
        {
            object Result = _Master.SaveRequestAction(RequestAction);
            return Ok(Result);
        }
    }
}
