
using System.ComponentModel.DataAnnotations;

namespace Portfoilio_Server.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string? Url { get; set; }

        [Required]
        public string StartDate { get; set; } = string.Empty;
        
        public string? EndDate { get; set; }

        [Required]
        public string Language { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Planning";

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
    }
}