using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class NameEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İsim zorunludur.")]
        [StringLength(50, ErrorMessage = "İsim 50 karakterden uzun olamaz.")]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
