using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    public class HelloViewModel
    {
        [Required(ErrorMessage = "İsim zorunludur")]
        public string Name { get; set; }

        public int? EditId { get; set; }

        public List<NameEntity> Names { get; set; } = new();
    }
}
