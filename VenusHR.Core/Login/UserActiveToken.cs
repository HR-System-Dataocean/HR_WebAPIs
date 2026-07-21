using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VenusHR.Core.Login
{
    /// <summary>
    /// Holds the single currently-valid token (jti) per employee.
    /// A token is valid only while its jti matches this record,
    /// so logging in again or logging out invalidates the previous token.
    /// </summary>
    [Table("UserActiveTokens")]
    public class UserActiveToken
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Jti { get; set; } = string.Empty;

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    }
}
