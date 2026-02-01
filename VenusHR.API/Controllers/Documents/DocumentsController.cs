using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using VenusHR.Application.Common.Interfaces.Documents;
using WorkFlow_EF;

namespace VenusHR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentsService _documentsService;

        public DocumentsController(IDocumentsService documentsService)
        {
            _documentsService = documentsService;
        }

        [HttpPost("upload-attachment")]
        public async Task<ActionResult<object>> UploadAttachment(
            [FromForm] UploadAttachmentRequest request,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (request == null || request.File == null || request.File.Length == 0)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "الملف مطلوب" : "File is required"
                    });
                }

                if (request.DocumentId <= 0 || request.ObjectId <= 0 || request.RecordId <= 0)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "معرّفات المستند غير صحيحة" : "Invalid document identifiers"
                    });
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

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = (Lang == 1) ? "حدث خطأ أثناء رفع الملف" : "Error uploading file",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("add-document")]
        public ActionResult<object> AddDocument(
            [FromBody] AddDocumentRequest request,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "بيانات الطلب غير صحيحة" : "Invalid request data"
                    });
                }

                if (request.DocumentId <= 0 || request.ObjectId <= 0 || request.RecordId <= 0)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "معرّفات المستند غير صحيحة" : "Invalid document identifiers"
                    });
                }

                if (string.IsNullOrEmpty(request.DocumentNumber))
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "رقم المستند مطلوب" : "Document number is required"
                    });
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

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = (Lang == 1) ? "حدث خطأ أثناء إضافة المستند" : "Error adding document",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("GetAttachments")]
        public ActionResult<object> GetAttachments(
            [FromQuery] int ObjectId,
            [FromQuery] long RecordId,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (ObjectId <= 0 || RecordId <= 0)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "معرّفات السجل غير صحيحة" : "Invalid record identifiers"
                    });
                }

                var result = _documentsService.GetAttachments(ObjectId, RecordId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = (Lang == 1) ? "حدث خطأ أثناء جلب المرفقات" : "Error retrieving attachments",
                    Error = ex.Message
                });
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
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "معرّف المرفق غير صحيح" : "Invalid attachment ID"
                    });
                }

                var result = _documentsService.GetAttachmentInfo(attachmentId);

                if (result is GeneralOutputClass<object> output && output.ErrorCode == 1)
                {
                    // استخدام reflection بدلاً من dynamic
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

                return NotFound(new
                {
                    Status = false,
                    Message = (Lang == 1) ? "الملف غير موجود" : "File not found"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = (Lang == 1) ? "حدث خطأ أثناء تحميل الملف" : "Error downloading file",
                    Error = ex.Message
                });
            }
        }

        [HttpDelete("delete-attachment/{attachmentId}")]
        public ActionResult<object> DeleteAttachment(
            int attachmentId,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (attachmentId <= 0)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "معرّف المرفق غير صحيح" : "Invalid attachment ID"
                    });
                }

                var result = _documentsService.DeleteAttachment(attachmentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = (Lang == 1) ? "حدث خطأ أثناء حذف المرفق" : "Error deleting attachment",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("document-details")]
        public ActionResult<object> GetDocumentDetails(
            [FromQuery] int ObjectId,
            [FromQuery] int RecordId,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (ObjectId <= 0 || RecordId <= 0)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "معرّفات السجل غير صحيحة" : "Invalid record identifiers"
                    });
                }

                var result = _documentsService.GetDocumentDetails(ObjectId, RecordId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = (Lang == 1) ? "حدث خطأ أثناء جلب المستندات" : "Error retrieving documents",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("document-types")]
        public ActionResult<object> GetDocumentTypes(
            [FromQuery] bool? isForCompany = null,
            [FromQuery] int? documentTypesGroupId = null,
            [FromQuery] int Lang = 0)
        {
            try
            {
                var result = _documentsService.GetDocumentTypes(isForCompany, documentTypesGroupId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = (Lang == 1) ? "حدث خطأ أثناء جلب أنواع المستندات" : "Error retrieving document types",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("document-detail/{documentDetailId}")]
        public ActionResult<object> GetDocumentDetail(
            int documentDetailId,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (documentDetailId <= 0)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "معرّف المستند غير صحيح" : "Invalid document ID"
                    });
                }

                var result = _documentsService.GetDocumentDetail(documentDetailId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = (Lang == 1) ? "حدث خطأ أثناء جلب تفاصيل المستند" : "Error retrieving document details",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("update-document/{documentDetailId}")]
        public ActionResult<object> UpdateDocumentDetail(
            int documentDetailId,
            [FromBody] UpdateDocumentRequest request,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (documentDetailId <= 0)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "معرّف المستند غير صحيح" : "Invalid document ID"
                    });
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

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = (Lang == 1) ? "حدث خطأ أثناء تحديث المستند" : "Error updating document",
                    Error = ex.Message
                });
            }
        }

        [HttpDelete("delete-document/{documentDetailId}")]
        public ActionResult<object> DeleteDocumentDetail(
            int documentDetailId,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (documentDetailId <= 0)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "معرّف المستند غير صحيح" : "Invalid document ID"
                    });
                }

                var result = _documentsService.DeleteDocumentDetail(documentDetailId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = (Lang == 1) ? "حدث خطأ أثناء حذف المستند" : "Error deleting document",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("attachment-info/{attachmentId}")]
        public ActionResult<object> GetAttachmentInfo(
            int attachmentId,
            [FromQuery] int Lang = 0)
        {
            try
            {
                if (attachmentId <= 0)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "معرّف المرفق غير صحيح" : "Invalid attachment ID"
                    });
                }

                var result = _documentsService.GetAttachmentInfo(attachmentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = (Lang == 1) ? "حدث خطأ أثناء جلب معلومات المرفق" : "Error retrieving attachment info",
                    Error = ex.Message
                });
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