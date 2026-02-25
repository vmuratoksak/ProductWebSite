using SatisSitesi.Models.Entities;

namespace SatisSitesi.Services.Interfaces
{
    public interface IAuthService
    {
        void Register(UserEntity user);
        UserEntity Login(string email, string password);
    }
}
