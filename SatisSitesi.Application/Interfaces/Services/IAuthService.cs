using SatisSitesi.Domain.Entities;

namespace SatisSitesi.Application.Interfaces.Services
{
    public interface IAuthService
    {
        void Register(UserEntity user);
        UserEntity Login(string email, string password);
        UserEntity GetUserById(string userId);
        void UpdateProfile(string userId, string username, string email);
        void ChangePassword(string userId, string currentPassword, string newPassword);
        void DeleteAccount(string userId);
    }
}
