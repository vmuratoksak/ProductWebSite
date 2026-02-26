using System.Collections.Generic;
using SatisSitesi.Domain.Entities;

namespace SatisSitesi.Application.Models
{
    public class NameIndexModel
    {
        public List<NameEntity> Names { get; set; }

        public string LastUpdatedName { get; set; }
        public string Search { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
