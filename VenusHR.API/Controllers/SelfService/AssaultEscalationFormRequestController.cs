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
    public class AssaultEscalationFormRequestController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IAssaultEscalationFormRequestService _assaultEscalationFormRequestService;

        public AssaultEscalationFormRequestController(IAssaultEscalationFormRequestService assaultEscalationFormRequestService, ApplicationDBContext context)
        {
            _context = context;
            _assaultEscalationFormRequestService = assaultEscalationFormRequestService;
        }

        [HttpGet, Route("GetAll")]
        public ActionResult<ApiResponse<object>> GetAll([FromQuery] int Lang = 0)
        {
            try
            {
                var result = _assaultEscalationFormRequestService.GetAll(Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب طلبات تصعيد الاعتداء" : "Failed to retrieve assault escalation form requests";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب طلبات تصعيد الاعتداء بنجاح" : "Assault escalation form requests retrieved successfully";
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
                var result = _assaultEscalationFormRequestService.GetById(id, Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب طلب تصعيد الاعتداء" : "Failed to retrieve assault escalation form request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    if (output.ResultObject == null)
                    {
                        var notFoundMsg = Lang == 1 ? "طلب تصعيد الاعتداء غير موجود" : "Assault escalation form request not found";
                        return NotFound(ApiResponse<object>.Fail(notFoundMsg));
                    }

                    var successMsg = Lang == 1 ? "تم جلب طلب تصعيد الاعتداء بنجاح" : "Assault escalation form request retrieved successfully";
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
                var result = _assaultEscalationFormRequestService.GetByEmployeeId(employeeId, Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب طلبات تصعيد الاعتداء للموظف" : "Failed to retrieve employee assault escalation form requests";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب طلبات تصعيد الاعتداء للموظف بنجاح" : "Employee assault escalation form requests retrieved successfully";
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
        public ActionResult<ApiResponse<object>> Create([FromBody] SS_AssaultEscalationFormRequest request, [FromQuery] int Lang = 0)
        {
            try
            {
                var result = _assaultEscalationFormRequestService.Create(request);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل إنشاء طلب تصعيد الاعتداء" : "Failed to create assault escalation form request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم إنشاء طلب تصعيد الاعتداء بنجاح" : "Assault escalation form request created successfully";
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
        public ActionResult<ApiResponse<object>> Update([FromBody] SS_AssaultEscalationFormRequest request, [FromQuery] int Lang = 0)
        {
            try
            {
                var result = _assaultEscalationFormRequestService.Update(request);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل تعديل طلب تصعيد الاعتداء" : "Failed to update assault escalation form request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم تعديل طلب تصعيد الاعتداء بنجاح" : "Assault escalation form request updated successfully";
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
                var result = _assaultEscalationFormRequestService.Delete(id);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل حذف طلب تصعيد الاعتداء" : "Failed to delete assault escalation form request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم حذف طلب تصعيد الاعتداء بنجاح" : "Assault escalation form request deleted successfully";
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
