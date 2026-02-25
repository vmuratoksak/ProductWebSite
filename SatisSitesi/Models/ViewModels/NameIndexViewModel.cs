using System.Collections.Generic;
using SatisSitesi.Models.Entities;

namespace SatisSitesi.Models.ViewModels
{
    public class NameIndexViewModel
    {
        public List<NameEntity> Names { get; set; }

        public string LastUpdatedName { get; set; }
        public string Search { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
