using WebApplication1.Services.Interfaces;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Models.Entities;
using System;
using System.Linq;

namespace WebApplication1.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<UserEntity> _userRepo;

        public AuthService(IRepository<UserEntity> userRepo)
        {
            _userRepo = userRepo;
        }

        public void Register(UserEntity user)
        {
            var users = _userRepo.GetAll();

            if (users.Any(x => x.Email == user.Email))
                throw new Exception("Bu email zaten kayıtlı.");

            if (string.IsNullOrWhiteSpace(user.Password))
                throw new Exception("Şifre boş olamaz.");

            user.Role = "User"; // default role
            user.CreatedAt = DateTime.Now;

            _userRepo.Insert(user);
        }

        public UserEntity Login(string email, string password)
        {
            return _userRepo.GetAll()
                .FirstOrDefault(x => x.Email == email && x.Password == password);
        }
    }
}