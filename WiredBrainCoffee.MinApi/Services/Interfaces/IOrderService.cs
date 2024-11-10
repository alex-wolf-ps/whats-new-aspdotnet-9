using WiredBrainCoffee.Models;

namespace WiredBrainCoffee.MinApi.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> GetOrderById(int id);
        Task<List<Order>> GetOrders();
    }
}
