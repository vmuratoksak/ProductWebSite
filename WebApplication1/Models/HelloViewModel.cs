using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class HelloViewModel
    {
        [Required(ErrorMessage = "İsim zorunludur")]
        public string? Name { get; set; }

        public string? Message { get; set; }

        public DateTime Date { get; set; }
    }
}
