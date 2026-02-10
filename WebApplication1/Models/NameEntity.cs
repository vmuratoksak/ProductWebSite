using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class NameEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İsim zorunludur")]
        [StringLength(50, ErrorMessage = "En fazla 50 karakter girebilirsiniz")]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
