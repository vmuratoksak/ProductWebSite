using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class HelloIndexViewModel
    {
        public NameEntity NewName { get; set; } = new();
        public List<NameEntity> Names { get; set; } = new();
    }
}
