using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class HelloViewModel
    {
        [Required(ErrorMessage = "İsim zorunludur")]
        public string Name { get; set; }

        public int? EditIndex { get; set; }

        public DateTime Date { get; set; }

        public List<string> Names { get; set; } = new();
    }
}
