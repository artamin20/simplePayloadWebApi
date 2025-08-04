using System.ComponentModel.DataAnnotations;

namespace simplePayloadWebApi.Models
{
    public class UserInteraction
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? Country { get; set; }

        public string? UserAgent { get; set; }

        public string? Device { get; set; }

        public string? OS { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
