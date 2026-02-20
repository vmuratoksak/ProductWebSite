namespace WebApplication1.Models.ViewModels
{
    public class CartViewModel
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice => Price * Quantity;
    }
}