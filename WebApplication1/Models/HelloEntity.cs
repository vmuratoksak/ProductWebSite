using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class HelloEntity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
