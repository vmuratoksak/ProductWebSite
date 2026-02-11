using System.Collections.Generic;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    public class HelloIndexViewModel
    {
        public string NewName { get; set; }
        public string SearchTerm { get; set; }
        public List<NameEntity> Names { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}
