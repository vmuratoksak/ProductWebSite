using SatisSitesi.Domain.Entities;

namespace SatisSitesi.Application.Interfaces.Services
{
    public interface IAuthService
    {
        void Register(UserEntity user);
        UserEntity Login(string email, string password);
    }
}
