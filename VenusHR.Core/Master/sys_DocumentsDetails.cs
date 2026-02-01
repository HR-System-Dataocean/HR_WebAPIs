using System;
using System.ComponentModel.DataAnnotations;

namespace VenusHR.Core.Master
{
    public class sys_DocumentsDetails
    {
        [Key]
        public int ID { get; set; }
        public int? DocumentID { get; set; }
        public int ObjectID { get; set; }
        public int RecordID { get; set; }
        public string? DocumentNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public int? IssuedCityID { get; set; }
        public DateTime? LastRenewalDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Remarks { get; set; }
        public int? RegUserID { get; set; }
        public int? RegComputerID { get; set; }
        public DateTime RegDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? ExpiryDate_D { get; set; }
        public string? IssueDate_D { get; set; }
        public string? LastRenewalDate_D { get; set; }
        public string? CombID { get; set; }
        public string? ReferenceNumber { get; set; }
    }
}