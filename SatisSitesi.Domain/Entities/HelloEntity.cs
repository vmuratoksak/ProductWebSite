using System;
using System.ComponentModel.DataAnnotations;

namespace SatisSitesi.Domain.Entities
{
    public class HelloEntity : BaseEntity
    {
        [Required]
        public string Name { get; set; }
    }
}
