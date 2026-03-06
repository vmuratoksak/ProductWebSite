using SatisSitesi.Domain.Entities;
using SatisSitesi.Application.Models;
using SatisSitesi.Application.Interfaces.Repositories;
using SatisSitesi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SatisSitesi.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<UserEntity> _repository;

        public CustomerService(IRepository<UserEntity> repository)
        {
            _repository = repository;
        }

        public CustomerIndexModel GetPaged(string search, string role, string status, int page, int pageSize)
        {
            var query = _repository.GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(x => x.Username.ToLower().Contains(lowerSearch) || x.Email.ToLower().Contains(lowerSearch));
            }

            if (!string.IsNullOrEmpty(role) && role != "All")
            {
                query = query.Where(x => x.Role == role);
            }

            var totalCount = query.Count();

            var users = query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var usersList = users.Select(user => new UserViewModel
            {
                Id = user.Id,
                Initials = !string.IsNullOrEmpty(user.Username) && user.Username.Length >= 1 ? user.Username.Substring(0, 1).ToUpper() : "U",
                Name = user.Username ?? "Unknown",
                Email = user.Email ?? "",
                Role = user.Role ?? "User",
                Status = "Active", // Static for now as per image logic
                StatusColor = "text-success",
                AddedDate = user.CreatedAt
            }).ToList();

            return new CustomerIndexModel
            {
                Users = usersList,
                Search = search,
                RoleFilter = role ?? "All",
                StatusFilter = status ?? "Active",
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                TotalUsers = totalCount
            };
        }

        public UserEntity GetById(string id)
        {
            return _repository.GetById(id);
        }

        public void Update(UserEntity model)
        {
            var existing = _repository.GetById(model.Id);
            if (existing != null)
            {
                existing.Username = model.Username;
                existing.Email = model.Email;
                existing.Role = model.Role;
                existing.UpdatedAt = DateTime.Now;
                _repository.Update(existing.Id, existing);
            }
        }

        public void Delete(string id)
        {
            _repository.Delete(id);
        }

        public void Add(UserEntity user)
        {
            if (user.CreatedAt == default)
                user.CreatedAt = DateTime.Now;
            
            _repository.Insert(user);
        }
    }
}
