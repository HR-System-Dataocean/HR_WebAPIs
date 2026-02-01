using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VenusHR.Application.Common.Interfaces.SelfService;
using VenusHR.Core.Master;
using VenusHR.Core.SelfService;
using VenusHR.Infrastructure.Presistence;

namespace VenusHR.API.Controllers.SelfService
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnualVacationRequestController : ControllerBase
    {
        private ApplicationDBContext _context;
        private readonly IAnnualVacationRequestService _AnnualVacationRequestService;
        public AnnualVacationRequestController(IAnnualVacationRequestService AnnualVacationRequestService, ApplicationDBContext Context)
        {
            _context = Context;
            _AnnualVacationRequestService = AnnualVacationRequestService;

        }
        [HttpPost("SaveRequest")]
        public ActionResult<List<object>> SaveRequest([FromBody] dynamic requestData, [FromQuery] string requestType)
        {
            var result = _AnnualVacationRequestService.SaveSelfServiceRequest(requestData, requestType);
            return Ok(result);
        }
        [HttpGet, Route("GetAnnualVacsBalancesByEMP/{EmpID}/{ToDate}")]
        public ActionResult<Hrs_EmployeesVacations> GetEmpAnnualVacationRemainingBalanceByEmpCode(int EmpID, DateTime ToDate)
        {

            var Result = _AnnualVacationRequestService.GetAnnualVacsBalancesByEMP(EmpID, ToDate);
            return Ok(Result);
        }
        [HttpGet, Route("api/GeatServicePeriod/{EmployeeID}/{EndServiceDate}/")]
        public ActionResult<Hrs_EmployeesVacations> GeatServicePeriod(int EmployeeID, DateTime EndServiceDate)
        {

            var Result = _AnnualVacationRequestService.GeatServicePeriod(EmployeeID, EndServiceDate);
            return Ok(Result);
        }

 


        [HttpGet, Route("GetRequestStages/{RequestSerial}/{FormCode}/{Lang}")]
        public ActionResult<Hrs_EmployeesVacations> GetRequestStages(int RequestSerial, string FormCode, int Lang)
        {

            var Result = _AnnualVacationRequestService.GetRequestStages(RequestSerial, FormCode, Lang);
            return Ok(Result);
        }

        [HttpGet, Route("GetSelfServiceRequestDetails/{requestType}/{ReauestID}/{Lang}/{ConfigID}")]
        public ActionResult  GetSelfServiceRequestDetails(string requestType, int ReauestID, int Lang, int ConfigID)
        {

            var Result = _AnnualVacationRequestService.GetSelfServiceRequestDetails(requestType, ReauestID, Lang,ConfigID);
            return Ok(Result);
        }


        [HttpGet, Route("GetMySelfServiceRequestDetails/{requestType}/{ReauestID}/{Lang}")]
        public ActionResult GetMySelfServiceRequestDetails(string requestType, int ReauestID, int Lang)
        {

            var Result = _AnnualVacationRequestService.GetMySelfServiceRequestDetails(requestType, ReauestID, Lang);
            return Ok(Result);
        }

         [HttpGet, Route("GetAnnualVacationRequestByID/{RequestSerial}/{Lang}")]

        public ActionResult<SS_VacationRequest> GetAnnualVacationRequestByID(int RequestSerial, int Lang)
        {
            var Result = _AnnualVacationRequestService.GetAnnualVacationRequestByID(RequestSerial,Lang);
            return Ok(Result);
        }
        [HttpGet("GetEmployeeRequests/{employeeId}/{Lang}")]

        public ActionResult GetEmployeeRequests(int employeeId, int Lang)
        {
            var Result = _AnnualVacationRequestService.GetEmployeeRequests(employeeId, Lang);
            return Ok(Result);
        }
    }
}
