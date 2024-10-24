using System.ComponentModel.DataAnnotations;

namespace PasteTrue.DTOs.Paste
{
    public class CreatePasteDto
    {
        [Required]
        [MaxLength(10000)]
        [MinLength(1)]

        public string Content { get; set; }

        [Required]
        [MaxLength(150)]
        [MinLength(1)]
        public string Title { get; set; }

        [MaxLength(100)]
        [MinLength(10)]
        public string? Password { get; set; }
    }
}
