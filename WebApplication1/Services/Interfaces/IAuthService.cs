using WebApplication1.Models.Entities;

namespace WebApplication1.Services.Interfaces
{
    public interface IAuthService
    {
        void Register(UserEntity user);
        UserEntity Login(string email, string password);
    }
}
