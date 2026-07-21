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
    public class AnnualTicketRelatedRequestsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IAnnualTicketRelatedRequestsService _annualTicketRelatedRequestsService;

        public AnnualTicketRelatedRequestsController(IAnnualTicketRelatedRequestsService annualTicketRelatedRequestsService, ApplicationDBContext context)
        {
            _context = context;
            _annualTicketRelatedRequestsService = annualTicketRelatedRequestsService;
        }

        [HttpGet, Route("GetAll")]
        public ActionResult<ApiResponse<object>> GetAll([FromQuery] int Lang = 0)
        {
            try
            {
                var result = _annualTicketRelatedRequestsService.GetAll(Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب طلبات التذاكر السنوية" : "Failed to retrieve annual ticket requests";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب طلبات التذاكر السنوية بنجاح" : "Annual ticket requests retrieved successfully";
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
                var result = _annualTicketRelatedRequestsService.GetById(id, Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب طلب التذكرة السنوية" : "Failed to retrieve annual ticket request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    if (output.ResultObject == null)
                    {
                        var notFoundMsg = Lang == 1 ? "طلب التذكرة السنوية غير موجود" : "Annual ticket request not found";
                        return NotFound(ApiResponse<object>.Fail(notFoundMsg));
                    }

                    var successMsg = Lang == 1 ? "تم جلب طلب التذكرة السنوية بنجاح" : "Annual ticket request retrieved successfully";
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
                var result = _annualTicketRelatedRequestsService.GetByEmployeeId(employeeId, Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب طلبات التذاكر السنوية للموظف" : "Failed to retrieve employee annual ticket requests";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب طلبات التذاكر السنوية للموظف بنجاح" : "Employee annual ticket requests retrieved successfully";
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
        public ActionResult<ApiResponse<object>> Create([FromBody] SS_AnnualTicketRelatedRequests request, [FromQuery] int Lang = 0)
        {
            try
            {
                var result = _annualTicketRelatedRequestsService.Create(request);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل إنشاء طلب التذكرة السنوية" : "Failed to create annual ticket request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم إنشاء طلب التذكرة السنوية بنجاح" : "Annual ticket request created successfully";
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
        public ActionResult<ApiResponse<object>> Update([FromBody] SS_AnnualTicketRelatedRequests request, [FromQuery] int Lang = 0)
        {
            try
            {
                var result = _annualTicketRelatedRequestsService.Update(request);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل تعديل طلب التذكرة السنوية" : "Failed to update annual ticket request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم تعديل طلب التذكرة السنوية بنجاح" : "Annual ticket request updated successfully";
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
                var result = _annualTicketRelatedRequestsService.Delete(id);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل حذف طلب التذكرة السنوية" : "Failed to delete annual ticket request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم حذف طلب التذكرة السنوية بنجاح" : "Annual ticket request deleted successfully";
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
