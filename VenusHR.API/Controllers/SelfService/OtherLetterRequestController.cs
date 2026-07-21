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
    public class OtherLetterRequestController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IOtherLetterRequestService _otherLetterRequestService;

        public OtherLetterRequestController(IOtherLetterRequestService otherLetterRequestService, ApplicationDBContext context)
        {
            _context = context;
            _otherLetterRequestService = otherLetterRequestService;
        }

        [HttpGet, Route("GetAll")]
        public ActionResult<ApiResponse<object>> GetAll([FromQuery] int Lang = 0)
        {
            try
            {
                var result = _otherLetterRequestService.GetAll(Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب طلبات الخطابات الأخرى" : "Failed to retrieve other letter requests";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب طلبات الخطابات الأخرى بنجاح" : "Other letter requests retrieved successfully";
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
                var result = _otherLetterRequestService.GetById(id, Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب طلب الخطاب الآخر" : "Failed to retrieve other letter request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    if (output.ResultObject == null)
                    {
                        var notFoundMsg = Lang == 1 ? "طلب الخطاب الآخر غير موجود" : "Other letter request not found";
                        return NotFound(ApiResponse<object>.Fail(notFoundMsg));
                    }

                    var successMsg = Lang == 1 ? "تم جلب طلب الخطاب الآخر بنجاح" : "Other letter request retrieved successfully";
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
                var result = _otherLetterRequestService.GetByEmployeeId(employeeId, Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب طلبات الخطابات الأخرى للموظف" : "Failed to retrieve employee other letter requests";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب طلبات الخطابات الأخرى للموظف بنجاح" : "Employee other letter requests retrieved successfully";
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
        public ActionResult<ApiResponse<object>> Create([FromBody] SS_OtherLetterRequest request, [FromQuery] int Lang = 0)
        {
            try
            {
                var result = _otherLetterRequestService.Create(request);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل إنشاء طلب الخطاب الآخر" : "Failed to create other letter request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم إنشاء طلب الخطاب الآخر بنجاح" : "Other letter request created successfully";
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
        public ActionResult<ApiResponse<object>> Update([FromBody] SS_OtherLetterRequest request, [FromQuery] int Lang = 0)
        {
            try
            {
                var result = _otherLetterRequestService.Update(request);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل تعديل طلب الخطاب الآخر" : "Failed to update other letter request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم تعديل طلب الخطاب الآخر بنجاح" : "Other letter request updated successfully";
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
                var result = _otherLetterRequestService.Delete(id);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل حذف طلب الخطاب الآخر" : "Failed to delete other letter request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم حذف طلب الخطاب الآخر بنجاح" : "Other letter request deleted successfully";
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
