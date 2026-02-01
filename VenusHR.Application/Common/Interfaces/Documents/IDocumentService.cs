using System;
using System.IO;

namespace VenusHR.Application.Common.Interfaces.Documents
{
    public interface IDocumentsService
    {
        // حفظ المرفق والمستند معاً
        object SaveAttachment(int documentId, int objectId, long recordId, Stream fileStream,
                             string fileName, string engName, string arbName,
                             DateTime? issueDate = null, int? issuedCityId = null,
                             DateTime? expiryDate = null, string? documentNumber = null,
                             string? referenceNumber = null, DateTime? lastRenewalDate = null,
                             string? folderName = null);

        // جلب المرفقات الخاصة بسجل معين
        object GetAttachments(int objectId, long recordId);

        // حذف مرفق
        object DeleteAttachment(int attachmentId);

        // جبل معلومات ملف معين
        object GetAttachmentInfo(int attachmentId);

        // إضافة مستند جديد (بدون مرفق)
        object AddDocumentDetail(int documentId, int objectId, int recordId, string documentNumber,
                                 DateTime? issueDate, int? issuedCityId, DateTime? expiryDate,
                                 string? referenceNumber = null, DateTime? lastRenewalDate = null);

        // جلب مستندات سجل معين
        object GetDocumentDetails(int objectId, int recordId);

        // جلب تفاصيل مستند معين
        object GetDocumentDetail(int documentDetailId);

        // تحديث مستند
        object UpdateDocumentDetail(int documentDetailId, string? documentNumber = null,
                                    DateTime? issueDate = null, int? issuedCityId = null,
                                    DateTime? expiryDate = null, DateTime? lastRenewalDate = null,
                                    string? referenceNumber = null);

        // حذف مستند
        object DeleteDocumentDetail(int documentDetailId);

        // جلب أنواع المستندات
        object GetDocumentTypes(bool? isForCompany = null, int? documentTypesGroupId = null);
    }
}