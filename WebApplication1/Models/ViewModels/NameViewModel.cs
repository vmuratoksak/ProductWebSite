using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.Entities;

namespace WebApplication1.Models.ViewModels
{
    public class NameViewModel
    {
        [Required(ErrorMessage = "İsim zorunludur")]
        public string Name { get; set; }

        public int? EditId { get; set; }

        public List<NameEntity> Names { get; set; } = new();
    }
}
