namespace WebApplication1.Services.Interfaces
{
    public interface IOrderService
    {
        void Checkout(string userId, string userEmail);
    }
}
