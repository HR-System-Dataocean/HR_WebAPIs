using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VenusHR.Core.Login
{
    [Table("RevokedTokens")]
    public class RevokedToken
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(1000)]
        public string Token { get; set; } = string.Empty;
        
        public DateTime RevokedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ExpiresAt { get; set; }
        
        [MaxLength(100)]
        public string? Reason { get; set; }
    }
}
