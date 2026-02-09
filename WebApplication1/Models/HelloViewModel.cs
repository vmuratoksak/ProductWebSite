using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class HelloViewModel
    {
        public string Message { get; set; }
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Ad alanı boş bırakılamaz")]
        public string Name { get; set; }
    }
}
