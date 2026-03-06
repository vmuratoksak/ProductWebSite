using System;
using System.Collections.Generic;

namespace SatisSitesi.Application.Models
{
    public class CustomerIndexModel
    {
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
        public string Search { get; set; }
        public string RoleFilter { get; set; }
        public string StatusFilter { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalUsers { get; set; }
    }

    public class UserViewModel
    {
        public string Id { get; set; }
        public string Initials { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string StatusColor { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
