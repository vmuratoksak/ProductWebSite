using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class HelloIndexViewModel
    {
        public List<NameEntity> Names { get; set; }

        public string LastUpdatedName { get; set; }
        public string Search { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
