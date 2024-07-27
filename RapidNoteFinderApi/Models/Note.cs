using System;
using System.ComponentModel.DataAnnotations;

namespace RapidNoteFinderApi.Models
{
    public class Note
    {
        [Key]
        public int? Id { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required]
        public string? Content { get; set; }

        [StringLength(100, ErrorMessage = "Associate cannot exceed 100 characters.")]
        public string? Associate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CreatedAt { get; set; }
    }
}
