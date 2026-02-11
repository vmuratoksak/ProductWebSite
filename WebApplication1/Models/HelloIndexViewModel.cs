using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class HelloIndexViewModel
    {
        public List<NameEntity> Names { get; set; }

        public NameEntity NewName { get; set; }

        public string SearchTerm { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}
