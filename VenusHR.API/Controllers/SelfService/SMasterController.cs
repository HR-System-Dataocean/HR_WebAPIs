using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VenusHR.Application.Common.Interfaces.SelfService;
using VenusHR.Core.Master;
using VenusHR.Core.SelfService;
using VenusHR.Infrastructure.Presistence;
using VenusHR.Infrastructure.Presistence.SelfService;
using VenusHR.API.Models;
using WorkFlow_EF;

namespace VenusHR.API.Controllers.SelfService
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public ActionResult<ApiResponse<object>> GetAllRequestTypes([FromQuery] int Lang = 0)
        {
            try
            {
                var Result = _Master.GetAllRequestTypes();

                if (Result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب أنواع الطلبات" : "Failed to retrieve request types";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب أنواع الطلبات بنجاح" : "Request types retrieved successfully";
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
        [HttpGet, Route("GetAllEmployees/{Lang}")]
        public ActionResult<ApiResponse<object>> GetAllEmployees(int Lang)
        {
            try
            {
                var Result = _Master.GetAllEmployees(Lang);

                if (Result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب الموظفين" : "Failed to retrieve employees";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب الموظفين بنجاح" : "Employees retrieved successfully";
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
        [HttpGet, Route("GetEmployeeByID/{id}")]
        public ActionResult<ApiResponse<object>> GetEmployeeByID(int id, int Lang)
        {
            try
            {
                object Result = _Master.GetEmployeeByID(id, Lang);

                if (Result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "الموظف غير موجود" : "Employee not found";
                        return NotFound(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب الموظف بنجاح" : "Employee retrieved successfully";
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

        [HttpGet, Route("GetUserNotificationCount/{EmployeeID}")]
        public ActionResult<ApiResponse<object>> GetUserNotificationCount(string EmployeeID, [FromQuery] int Lang = 0)
        {
            try
            {
                object Result = _Master.GetUserNotificationCount(EmployeeID);

                if (Result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب عدد الإشعارات" : "Failed to retrieve notification count";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب عدد الإشعارات بنجاح" : "Notification count retrieved successfully";
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
        [HttpGet, Route("GetAllVacationsTypes/{Lang}")]
        public ActionResult<ApiResponse<object>> GetAllVacationsTypes(int Lang)
        {
            try
            {
                object Result = _Master.GetAllVacationsTypes(Lang);

                if (Result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب أنواع الإجازات" : "Failed to retrieve vacation types";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب أنواع الإجازات بنجاح" : "Vacation types retrieved successfully";
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

        [HttpGet, Route("GetEndOfServiceAllResignationReason/{Lang}")]
        public ActionResult<ApiResponse<object>> GetEndOfServiceAllResignationReason(int Lang)
        {
            try
            {
                object Result = _Master.GetEndOfServiceAllResignationReason(Lang);

                if (Result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب أسباب الاستقالة" : "Failed to retrieve resignation reasons";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب أسباب الاستقالة بنجاح" : "Resignation reasons retrieved successfully";
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

        [HttpGet, Route("GetEndOfServiceAllExperienceRate/{Lang}")]
        public ActionResult<ApiResponse<object>> GetEndOfServiceAllExperienceRate(int Lang)
        {
            try
            {
                object Result = _Master.GetEndOfServiceAllExperienceRate(Lang);

                if (Result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب معدلات الخبرة" : "Failed to retrieve experience rates";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب معدلات الخبرة بنجاح" : "Experience rates retrieved successfully";
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
        [HttpGet, Route("GetAllPendingRequests/{EmployeeID}/{Lang}")]
        public ActionResult<ApiResponse<object>> GetAllPendingRequests(int EmployeeID, int Lang)
        {
            try
            {
                object Result = _Master.GetAllPendingRequests(EmployeeID, Lang);

                if (Result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب الطلبات المعلقة" : "Failed to retrieve pending requests";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب الطلبات المعلقة بنجاح" : "Pending requests retrieved successfully";
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

        [HttpGet, Route("GetEmployeeMonthlyTransactions/{employeeId}/{month}")]
        public ActionResult<ApiResponse<object>> GetEmployeeMonthlyTransactions(
            int employeeId,
            int month,
            [FromQuery] int? year = null,
            [FromQuery] int Lang = 0,
            [FromQuery] bool hideNotPaid = true)
        {
            try
            {
                var resolvedYear = year ?? DateTime.Now.Year;
                object result = _Master.GetEmployeeMonthlyTransactions(employeeId, month, resolvedYear, Lang, hideNotPaid);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب المعاملة الشهرية" : "Failed to retrieve monthly transaction";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب المعاملة الشهرية بنجاح" : "Monthly transaction retrieved successfully";
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

        [HttpGet, Route("GetEmployeeDependants/{employeeId}")]
        public ActionResult<ApiResponse<object>> GetEmployeeDependants(int employeeId, [FromQuery] int Lang = 0)
        {
            try
            {
                object result = _Master.GetEmployeeDependants(employeeId, Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب بيانات المرافقين" : "Failed to retrieve employee dependants";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب بيانات المرافقين بنجاح" : "Employee dependants retrieved successfully";
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

        [HttpGet, Route("GetEmployeeVacationBalance/{employeeId}")]
        public ActionResult<ApiResponse<object>> GetEmployeeVacationBalance(
            int employeeId,
            [FromQuery] int Lang = 0,
            [FromQuery] int? vacationTypeId = null,
            [FromQuery] DateTime? balanceDate = null,
            [FromQuery] DateTime? vacationEndDate = null,
            [FromQuery] int? vacationId = null)
        {
            try
            {
                object result = _Master.GetEmployeeVacationBalance(
                    employeeId, Lang, vacationTypeId, balanceDate, vacationEndDate, vacationId);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب رصيد الإجازة" : "Failed to retrieve vacation balance";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب رصيد الإجازة بنجاح" : "Vacation balance retrieved successfully";
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

        [HttpGet, Route("GetEmployeeHealthInsurance/{employeeId}")]
        public ActionResult<ApiResponse<object>> GetEmployeeHealthInsurance(int employeeId, [FromQuery] int Lang = 0)
        {
            try
            {
                object result = _Master.GetEmployeeHealthInsurance(employeeId, Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب بيانات التأمين الصحي" : "Failed to retrieve health insurance data";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب بيانات التأمين الصحي بنجاح" : "Health insurance data retrieved successfully";
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

        [HttpPost, Route("SaveRequestAction")]
        public ActionResult<ApiResponse<object>> SaveRequestAction(SS_RequestAction RequestAction, [FromQuery] int Lang = 0)
        {
            try
            {
                object Result = _Master.SaveRequestAction(RequestAction);

                if (Result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل حفظ إجراء الطلب" : "Failed to save request action";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم حفظ إجراء الطلب بنجاح" : "Request action saved successfully";
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
