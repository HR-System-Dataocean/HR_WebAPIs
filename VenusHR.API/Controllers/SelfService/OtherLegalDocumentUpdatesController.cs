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
    public class OtherLegalDocumentUpdatesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IOtherLegalDocumentUpdatesService _otherLegalDocumentUpdatesService;

        public OtherLegalDocumentUpdatesController(IOtherLegalDocumentUpdatesService otherLegalDocumentUpdatesService, ApplicationDBContext context)
        {
            _context = context;
            _otherLegalDocumentUpdatesService = otherLegalDocumentUpdatesService;
        }

        [HttpGet, Route("GetAll")]
        public ActionResult<ApiResponse<object>> GetAll([FromQuery] int Lang = 0)
        {
            try
            {
                var result = _otherLegalDocumentUpdatesService.GetAll(Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب طلبات تحديث المستندات القانونية" : "Failed to retrieve legal document update requests";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب طلبات تحديث المستندات القانونية بنجاح" : "Legal document update requests retrieved successfully";
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
                var result = _otherLegalDocumentUpdatesService.GetById(id, Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب طلب تحديث المستند القانوني" : "Failed to retrieve legal document update request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    if (output.ResultObject == null)
                    {
                        var notFoundMsg = Lang == 1 ? "طلب تحديث المستند القانوني غير موجود" : "Legal document update request not found";
                        return NotFound(ApiResponse<object>.Fail(notFoundMsg));
                    }

                    var successMsg = Lang == 1 ? "تم جلب طلب تحديث المستند القانوني بنجاح" : "Legal document update request retrieved successfully";
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
                var result = _otherLegalDocumentUpdatesService.GetByEmployeeId(employeeId, Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب طلبات تحديث المستندات القانونية للموظف" : "Failed to retrieve employee legal document update requests";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب طلبات تحديث المستندات القانونية للموظف بنجاح" : "Employee legal document update requests retrieved successfully";
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
        public ActionResult<ApiResponse<object>> Create([FromBody] SS_OtherLegalDocumentUpdates request, [FromQuery] int Lang = 0)
        {
            try
            {
                var result = _otherLegalDocumentUpdatesService.Create(request);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل إنشاء طلب تحديث المستند القانوني" : "Failed to create legal document update request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم إنشاء طلب تحديث المستند القانوني بنجاح" : "Legal document update request created successfully";
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
        public ActionResult<ApiResponse<object>> Update([FromBody] SS_OtherLegalDocumentUpdates request, [FromQuery] int Lang = 0)
        {
            try
            {
                var result = _otherLegalDocumentUpdatesService.Update(request);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل تعديل طلب تحديث المستند القانوني" : "Failed to update legal document update request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم تعديل طلب تحديث المستند القانوني بنجاح" : "Legal document update request updated successfully";
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
                var result = _otherLegalDocumentUpdatesService.Delete(id);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل حذف طلب تحديث المستند القانوني" : "Failed to delete legal document update request";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم حذف طلب تحديث المستند القانوني بنجاح" : "Legal document update request deleted successfully";
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
