using SatisSitesi.Application.Interfaces.Services;
using SatisSitesi.Application.Interfaces.Repositories;
using SatisSitesi.Domain.Entities;
using System;
using System.Linq;
using BCrypt.Net;

namespace SatisSitesi.Application.Services
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
            //password hashing 
            var existingUser = _userRepo.GetFirstOrDefault(x => x.Email == user.Email);

            if (existingUser != null)
                throw new Exception("Bu email zaten kayitli.");

            if (string.IsNullOrWhiteSpace(user.Password))
                throw new Exception("Sifre bos olamaz.");

            // Hash password before saving
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            user.Role = "User"; // default role
            user.CreatedAt = DateTime.Now;

            _userRepo.Insert(user);
        }

        public UserEntity Login(string email, string password)  
        {
            var user = _userRepo.GetFirstOrDefault(x => x.Email == email);

            if (user == null)
                return null;

            // Check if it's an old plain-text password account
            if (!user.Password.StartsWith("$2"))
            {
                if (user.Password == password)
                {
                    // It's a match! The user entered the correct old password.
                    // Automatically fix their account by hashing the password and updating DB.
                    user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                    _userRepo.Update(user.Id, user);
                    return user;
                }
                else
                {
                    return null;
                }
            }

            // Otherwise, it's a new (or already migrated) account. Verify password against stored hash.
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (!isPasswordValid)
                return null;

            return user;
        }

        public UserEntity GetUserById(string userId)
        {
            return _userRepo.GetById(userId);
        }

        public void UpdateProfile(string userId, string username, string email)
        {
            var user = _userRepo.GetById(userId);
            if (user == null) throw new Exception("Kullanıcı bulunamadı.");

            var existingUser = _userRepo.GetFirstOrDefault(x => x.Email == email && x.Id != userId);
            if (existingUser != null) throw new Exception("Bu email başka bir kullanıcı tarafından kullanılıyor.");

            user.Username = username;
            user.Email = email;
            _userRepo.Update(userId, user);
        }

        public void ChangePassword(string userId, string currentPassword, string newPassword)
        {
            var user = _userRepo.GetById(userId);
            if (user == null) throw new Exception("Kullanıcı bulunamadı.");

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
                throw new Exception("Mevcut şifre yanlış.");

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _userRepo.Update(userId, user);
        }

        public void DeleteAccount(string userId)
        {
            _userRepo.Delete(userId);
        }
    }
}
