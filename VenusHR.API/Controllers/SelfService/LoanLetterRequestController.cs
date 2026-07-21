using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VenusHR.Application.Common.Interfaces.SelfService;
using VenusHR.Core.SelfService;
using VenusHR.Infrastructure.Presistence;
using VenusHR.API.Models;
using WorkFlow_EF;

namespace VenusHR.API.Controllers.SelfService
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoanLetterRequestController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ILoanLetterRequestService _loanLetterRequestService;

        public LoanLetterRequestController(ILoanLetterRequestService loanLetterRequestService, ApplicationDBContext context)
        {
            _context = context;
            _loanLetterRequestService = loanLetterRequestService;
        }

        [HttpGet, Route("GetAll")]
        public ActionResult<ApiResponse<object>> GetAll([FromQuery] int Lang = 0)
        {
            try
            {
                var result = _loanLetterRequestService.GetAll(Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب طلبات خطاب القرض" : "Failed to retrieve loan letter requests";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب طلبات خطاب القرض بنجاح" : "Loan letter requests retrieved successfully";
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

        [HttpGet, Route("GetById/{id}")]
        public ActionResult<ApiResponse<object>> GetById(int id, [FromQuery] int Lang = 0)
        {
            try
            {
                var result = _loanLetterRequestService.GetById(id, Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب طلب خطاب القرض" : "Failed to retrieve loan letter request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    if (output.ResultObject == null)
                    {
                        var notFoundMsg = Lang == 1 ? "طلب خطاب القرض غير موجود" : "Loan letter request not found";
                        return NotFound(ApiResponse<object>.Fail(notFoundMsg));
                    }

                    var successMsg = Lang == 1 ? "تم جلب طلب خطاب القرض بنجاح" : "Loan letter request retrieved successfully";
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

        [HttpGet, Route("GetByEmployeeId/{employeeId}")]
        public ActionResult<ApiResponse<object>> GetByEmployeeId(int employeeId, [FromQuery] int Lang = 0)
        {
            try
            {
                var result = _loanLetterRequestService.GetByEmployeeId(employeeId, Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب طلبات خطاب القرض للموظف" : "Failed to retrieve employee loan letter requests";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب طلبات خطاب القرض للموظف بنجاح" : "Employee loan letter requests retrieved successfully";
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

        [HttpPost, Route("Create")]
        public ActionResult<ApiResponse<object>> Create([FromBody] SS_LoanLetterRequest request, [FromQuery] int Lang = 0)
        {
            try
            {
                var result = _loanLetterRequestService.Create(request);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل إنشاء طلب خطاب القرض" : "Failed to create loan letter request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم إنشاء طلب خطاب القرض بنجاح" : "Loan letter request created successfully";
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

        [HttpPut, Route("Update")]
        public ActionResult<ApiResponse<object>> Update([FromBody] SS_LoanLetterRequest request, [FromQuery] int Lang = 0)
        {
            try
            {
                var result = _loanLetterRequestService.Update(request);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل تعديل طلب خطاب القرض" : "Failed to update loan letter request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم تعديل طلب خطاب القرض بنجاح" : "Loan letter request updated successfully";
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

        [HttpDelete, Route("Delete/{id}")]
        public ActionResult<ApiResponse<object>> Delete(int id, [FromQuery] int Lang = 0)
        {
            try
            {
                var result = _loanLetterRequestService.Delete(id);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل حذف طلب خطاب القرض" : "Failed to delete loan letter request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم حذف طلب خطاب القرض بنجاح" : "Loan letter request deleted successfully";
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
