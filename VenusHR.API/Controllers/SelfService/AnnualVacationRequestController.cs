using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VenusHR.Application.Common.Interfaces.SelfService;
using VenusHR.Core.Master;
using VenusHR.Core.SelfService;
using VenusHR.Infrastructure.Presistence;
using VenusHR.API.Models;
using WorkFlow_EF;

namespace VenusHR.API.Controllers.SelfService
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public ActionResult<ApiResponse<object>> SaveRequest([FromBody] dynamic requestData, [FromQuery] string requestType, [FromQuery] int Lang = 0)
        {
            try
            {
                var result = _AnnualVacationRequestService.SaveSelfServiceRequest(requestData, requestType);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل حفظ الطلب" : "Failed to save request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم حفظ الطلب بنجاح" : "Request saved successfully";
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
        [HttpGet, Route("GetAnnualVacsBalancesByEMP/{EmpID}/{ToDate}")]
        public ActionResult<ApiResponse<object>> GetEmpAnnualVacationRemainingBalanceByEmpCode(int EmpID, DateTime ToDate, [FromQuery] int Lang = 0)
        {
            try
            {
                var Result = _AnnualVacationRequestService.GetAnnualVacsBalancesByEMP(EmpID, ToDate);

                if (Result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب رصيد الإجازات" : "Failed to retrieve vacation balance";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب رصيد الإجازات بنجاح" : "Vacation balance retrieved successfully";
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
        [HttpGet, Route("api/GeatServicePeriod/{EmployeeID}/{EndServiceDate}/")]
        public ActionResult<ApiResponse<object>> GeatServicePeriod(int EmployeeID, DateTime EndServiceDate, [FromQuery] int Lang = 0)
        {
            try
            {
                var Result = _AnnualVacationRequestService.GeatServicePeriod(EmployeeID, EndServiceDate);

                if (Result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب فترة الخدمة" : "Failed to retrieve service period";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب فترة الخدمة بنجاح" : "Service period retrieved successfully";
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

 


        [HttpGet, Route("GetRequestStages/{RequestSerial}/{FormCode}/{Lang}")]
        public ActionResult<ApiResponse<object>> GetRequestStages(int RequestSerial, string FormCode, int Lang)
        {
            try
            {
                var Result = _AnnualVacationRequestService.GetRequestStages(RequestSerial, FormCode, Lang);

                if (Result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب مراحل الطلب" : "Failed to retrieve request stages";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب مراحل الطلب بنجاح" : "Request stages retrieved successfully";
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

        [HttpGet, Route("GetSelfServiceRequestDetails/{requestType}/{ReauestID}/{Lang}/{ConfigID}")]
        public ActionResult<ApiResponse<object>> GetSelfServiceRequestDetails(string requestType, int ReauestID, int Lang, int ConfigID)
        {
            try
            {
                var Result = _AnnualVacationRequestService.GetSelfServiceRequestDetails(requestType, ReauestID, Lang, ConfigID);

                if (Result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب تفاصيل الطلب" : "Failed to retrieve request details";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب تفاصيل الطلب بنجاح" : "Request details retrieved successfully";
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


        [HttpGet, Route("GetMySelfServiceRequestDetails/{requestType}/{ReauestID}/{Lang}")]
        public ActionResult<ApiResponse<object>> GetMySelfServiceRequestDetails(string requestType, int ReauestID, int Lang)
        {
            try
            {
                var Result = _AnnualVacationRequestService.GetMySelfServiceRequestDetails(requestType, ReauestID, Lang);

                if (Result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب تفاصيل طلبي" : "Failed to retrieve my request details";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب تفاصيل طلبي بنجاح" : "My request details retrieved successfully";
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

         [HttpGet, Route("GetAnnualVacationRequestByID/{RequestSerial}/{Lang}")]

        public ActionResult<ApiResponse<object>> GetAnnualVacationRequestByID(int RequestSerial, int Lang)
        {
            try
            {
                var Result = _AnnualVacationRequestService.GetAnnualVacationRequestByID(RequestSerial, Lang);

                if (Result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب طلب الإجازة" : "Failed to retrieve vacation request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب طلب الإجازة بنجاح" : "Vacation request retrieved successfully";
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
        [HttpGet("GetEmployeeRequests/{employeeId}/{Lang}")]

        public ActionResult<ApiResponse<object>> GetEmployeeRequests(int employeeId, int Lang)
        {
            try
            {
                var Result = _AnnualVacationRequestService.GetEmployeeRequests(employeeId, Lang);

                if (Result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب طلبات الموظف" : "Failed to retrieve employee requests";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب طلبات الموظف بنجاح" : "Employee requests retrieved successfully";
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
