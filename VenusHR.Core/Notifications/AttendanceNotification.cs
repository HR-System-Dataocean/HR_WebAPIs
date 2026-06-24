namespace VenusHR.Core.Notifications
{
    public class AttendanceNotification
    {
        public long Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime NotificationDate { get; set; }
        public string NotificationType { get; set; } = "AttendanceAssignment";
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool SentViaSignalR { get; set; }
        public bool SentViaFcm { get; set; }
        public string DeliveryStatus { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SentAt { get; set; }
    }
}
