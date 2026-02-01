using System;
using System.Numerics;

namespace VenusHR.Core.Master
{
    public class sys_ObjectsAttachments
    {
        public int ID { get; set; }
        public int ObjectID { get; set; }
        public long RecordID { get; set; }
        public string? EngName { get; set; }
        public string? ArbName { get; set; }
        public string? ArbName4S { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool? IsImageView { get; set; }
        public bool? IsProfilePicture { get; set; }
        public int? RegUserID { get; set; }
        public int? RegComputerID { get; set; }
        public DateTime RegDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? FolderName { get; set; }
        public string? FileName { get; set; }
        public byte[]? MyImage { get; set; }
        public string? Remarks { get; set; }
        public string? CombID { get; set; }
        public bool? IsDGSignature { get; set; }

        // خاصية جديدة هنستخدمها للمسار
        public string FilePath => $"/{FolderName}/{FileName}";
    }
}