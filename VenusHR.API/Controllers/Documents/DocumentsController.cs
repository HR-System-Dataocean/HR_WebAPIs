using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using VenusHR.Application.Common.Interfaces.Documents;
using WorkFlow_EF;
using VenusHR.API.Models;

namespace VenusHR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentsService _documentsService;

        public DocumentsController(IDocumentsService documentsService)
        {
            _documentsService = documentsService;
        }

        [HttpPost("upload-attachment")]
        public async Task<ActionResult<ApiResponse<object>>> UploadAttachment(
            [FromForm] UploadAttachmentRequest request,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (request == null || request.File == null || request.File.Length == 0)
                {
                    var message = Lang == 1 ? "الملف مطلوب" : "File is required";
                    return BadRequest(ApiResponse<object>.Fail(message));
                }

                if (request.DocumentId <= 0 || request.ObjectId <= 0 || request.RecordId <= 0)
                {
                    var message = Lang == 1 ? "معرّفات المستند غير صحيحة" : "Invalid document identifiers";
                    return BadRequest(ApiResponse<object>.Fail(message));
                }

                using (var stream = request.File.OpenReadStream())
                {
                    var result = _documentsService.SaveAttachment(
                        request.DocumentId,
                        request.ObjectId,
                        request.RecordId,
                        stream,
                        request.File.FileName,
                        request.EngName ?? request.File.FileName,
                        request.ArbName ?? request.File.FileName,
                        request.IssueDate,
                        request.IssuedCityId,
                        request.ExpiryDate,
                        request.DocumentNumber,
                        request.ReferenceNumber,
                        request.LastRenewalDate,
                        request.FolderName
                    );

                    if (result is GeneralOutputClass<object> output)
                    {
                        if (output.ErrorCode == 0)
                        {
                            var message = Lang == 1 ? "فشل رفع الملف" : "Failed to upload file";
                            return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                        }

                        var successMsg = Lang == 1 ? "تم رفع الملف بنجاح" : "File uploaded successfully";
                        return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                    }

                    var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                    return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
                }
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ أثناء رفع الملف" : "Error uploading file";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpPost("add-document")]
        public ActionResult<ApiResponse<object>> AddDocument(
            [FromBody] AddDocumentRequest request,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (request == null)
                {
                    var message = Lang == 1 ? "بيانات الطلب غير صحيحة" : "Invalid request data";
                    return BadRequest(ApiResponse<object>.Fail(message));
                }

                if (request.DocumentId <= 0 || request.ObjectId <= 0 || request.RecordId <= 0)
                {
                    var message = Lang == 1 ? "معرّفات المستند غير صحيحة" : "Invalid document identifiers";
                    return BadRequest(ApiResponse<object>.Fail(message));
                }

                if (string.IsNullOrEmpty(request.DocumentNumber))
                {
                    var message = Lang == 1 ? "رقم المستند مطلوب" : "Document number is required";
                    return BadRequest(ApiResponse<object>.Fail(message));
                }

                var result = _documentsService.AddDocumentDetail(
                    request.DocumentId,
                    request.ObjectId,
                    request.RecordId,
                    request.DocumentNumber,
                    request.IssueDate,
                    request.IssuedCityId,
                    request.ExpiryDate,
                    request.ReferenceNumber,
                    request.LastRenewalDate
                );

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل إضافة المستند" : "Failed to add document";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم إضافة المستند بنجاح" : "Document added successfully";
                    return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                }

                var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ أثناء إضافة المستند" : "Error adding document";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("GetAttachments")]
        public ActionResult<ApiResponse<object>> GetAttachments(
            [FromQuery] int ObjectId,
            [FromQuery] long RecordId,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (ObjectId <= 0 || RecordId <= 0)
                {
                    var message = Lang == 1 ? "معرّفات السجل غير صحيحة" : "Invalid record identifiers";
                    return BadRequest(ApiResponse<object>.Fail(message));
                }

                var result = _documentsService.GetAttachments(ObjectId, RecordId);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب المرفقات" : "Failed to retrieve attachments";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب المرفقات بنجاح" : "Attachments retrieved successfully";
                    return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                }

                var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ أثناء جلب المرفقات" : "Error retrieving attachments";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("download/{attachmentId}")]
        public ActionResult DownloadAttachment(
            int attachmentId,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (attachmentId <= 0)
                {
                    var nmessage = Lang == 1 ? "معرّف المرفق غير صحيح" : "Invalid attachment ID";
                    return BadRequest(ApiResponse<object>.Fail(nmessage));
                }

                var result = _documentsService.GetAttachmentInfo(attachmentId);

                if (result is GeneralOutputClass<object> output && output.ErrorCode == 1)
                {
                    var resultObject = output.ResultObject;
                    if (resultObject != null)
                    {
                        var type = resultObject.GetType();
                        var fullPathProperty = type.GetProperty("FullPath");
                        var fileNameProperty = type.GetProperty("FileName");
                        var contentTypeProperty = type.GetProperty("ContentType");

                        if (fullPathProperty != null && fileNameProperty != null)
                        {
                            var filePath = fullPathProperty.GetValue(resultObject)?.ToString();
                            var fileName = fileNameProperty.GetValue(resultObject)?.ToString();
                            var contentType = contentTypeProperty?.GetValue(resultObject)?.ToString() ?? "application/octet-stream";

                            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                            {
                                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                                return File(fileBytes, contentType, fileName);
                            }
                        }
                    }
                }

                var message = Lang == 1 ? "الملف غير موجود" : "File not found";
                return NotFound(ApiResponse<object>.Fail(message, 1));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ أثناء تحميل الملف" : "Error downloading file";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpDelete("delete-attachment/{attachmentId}")]
        public ActionResult<ApiResponse<object>> DeleteAttachment(
            int attachmentId,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (attachmentId <= 0)
                {
                    var message = Lang == 1 ? "معرّف المرفق غير صحيح" : "Invalid attachment ID";
                    return BadRequest(ApiResponse<object>.Fail(message));
                }

                var result = _documentsService.DeleteAttachment(attachmentId);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل حذف المرفق" : "Failed to delete attachment";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم حذف المرفق بنجاح" : "Attachment deleted successfully";
                    return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                }

                var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ أثناء حذف المرفق" : "Error deleting attachment";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("document-details")]
        public ActionResult<ApiResponse<object>> GetDocumentDetails(
            [FromQuery] int ObjectId,
            [FromQuery] int RecordId,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (ObjectId <= 0 || RecordId <= 0)
                {
                    var message = Lang == 1 ? "معرّفات السجل غير صحيحة" : "Invalid record identifiers";
                    return BadRequest(ApiResponse<object>.Fail(message));
                }

                var result = _documentsService.GetDocumentDetails(ObjectId, RecordId);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب المستندات" : "Failed to retrieve documents";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب المستندات بنجاح" : "Documents retrieved successfully";
                    return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                }

                var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ أثناء جلب المستندات" : "Error retrieving documents";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("document-types")]
        public ActionResult<ApiResponse<object>> GetDocumentTypes(
            [FromQuery] bool? isForCompany = null,
            [FromQuery] int? documentTypesGroupId = null,
            [FromQuery] int Lang = 0)
        {
            try
            {
                var result = _documentsService.GetDocumentTypes(isForCompany, documentTypesGroupId);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب أنواع المستندات" : "Failed to retrieve document types";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب أنواع المستندات بنجاح" : "Document types retrieved successfully";
                    return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                }

                var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ أثناء جلب أنواع المستندات" : "Error retrieving document types";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("document-detail/{documentDetailId}")]
        public ActionResult<ApiResponse<object>> GetDocumentDetail(
            int documentDetailId,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (documentDetailId <= 0)
                {
                    var message = Lang == 1 ? "معرّف المستند غير صحيح" : "Invalid document ID";
                    return BadRequest(ApiResponse<object>.Fail(message));
                }

                var result = _documentsService.GetDocumentDetail(documentDetailId);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب تفاصيل المستند" : "Failed to retrieve document details";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب تفاصيل المستند بنجاح" : "Document details retrieved successfully";
                    return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                }

                var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ أثناء جلب تفاصيل المستند" : "Error retrieving document details";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpPut("update-document/{documentDetailId}")]
        public ActionResult<ApiResponse<object>> UpdateDocumentDetail(
            int documentDetailId,
            [FromBody] UpdateDocumentRequest request,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (documentDetailId <= 0)
                {
                    var message = Lang == 1 ? "معرّف المستند غير صحيح" : "Invalid document ID";
                    return BadRequest(ApiResponse<object>.Fail(message));
                }

                var result = _documentsService.UpdateDocumentDetail(
                    documentDetailId,
                    request.DocumentNumber,
                    request.IssueDate,
                    request.IssuedCityId,
                    request.ExpiryDate,
                    request.LastRenewalDate,
                    request.ReferenceNumber
                );

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل تحديث المستند" : "Failed to update document";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم تحديث المستند بنجاح" : "Document updated successfully";
                    return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                }

                var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ أثناء تحديث المستند" : "Error updating document";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpDelete("delete-document/{documentDetailId}")]
        public ActionResult<ApiResponse<object>> DeleteDocumentDetail(
            int documentDetailId,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (documentDetailId <= 0)
                {
                    var message = Lang == 1 ? "معرّف المستند غير صحيح" : "Invalid document ID";
                    return BadRequest(ApiResponse<object>.Fail(message));
                }

                var result = _documentsService.DeleteDocumentDetail(documentDetailId);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل حذف المستند" : "Failed to delete document";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم حذف المستند بنجاح" : "Document deleted successfully";
                    return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                }

                var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ أثناء حذف المستند" : "Error deleting document";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("attachment-info/{attachmentId}")]
        public ActionResult<ApiResponse<object>> GetAttachmentInfo(
            int attachmentId,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (attachmentId <= 0)
                {
                    var message = Lang == 1 ? "معرّف المرفق غير صحيح" : "Invalid attachment ID";
                    return BadRequest(ApiResponse<object>.Fail(message));
                }

                var result = _documentsService.GetAttachmentInfo(attachmentId);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب معلومات المرفق" : "Failed to retrieve attachment info";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب معلومات المرفق بنجاح" : "Attachment info retrieved successfully";
                    return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                }

                var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ أثناء جلب معلومات المرفق" : "Error retrieving attachment info";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }
    }

    // Request Models
    public class UploadAttachmentRequest
    {
        public int DocumentId { get; set; }
        public int ObjectId { get; set; }
        public long RecordId { get; set; }
        public IFormFile File { get; set; }
        public string? EngName { get; set; }
        public string? ArbName { get; set; }
        public DateTime? IssueDate { get; set; }
        public int? IssuedCityId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? DocumentNumber { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime? LastRenewalDate { get; set; }
        public string? FolderName { get; set; }
    }

    public class AddDocumentRequest
    {
        public int DocumentId { get; set; }
        public int ObjectId { get; set; }
        public int RecordId { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public int? IssuedCityId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime? LastRenewalDate { get; set; }
    }

    public class UpdateDocumentRequest
    {
        public string? DocumentNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public int? IssuedCityId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? LastRenewalDate { get; set; }
        public string? ReferenceNumber { get; set; }
    }
}