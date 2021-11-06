using System.ComponentModel.DataAnnotations;

namespace SimplzKeyGenVerifier.Models
{
    public class Log
    {
        public int LogId { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        [Required]
        public string? Hash { get; set; }
        public int LicenceId { get; set; }
    }
}