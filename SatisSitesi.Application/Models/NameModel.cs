using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SatisSitesi.Domain.Entities;

namespace SatisSitesi.Application.Models
{
    public class NameModel
    {
        [Required(ErrorMessage = "İsim zorunludur")]
        public string Name { get; set; }

        public int? EditId { get; set; }

        public List<NameEntity> Names { get; set; } = new();
    }
}
