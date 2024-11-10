using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using WiredBrainCoffee.MinApi.Services.Interfaces;
using WiredBrainCoffee.Models;

namespace WiredBrainCoffee.MinApi.Services
{
    public class OrderService : IOrderService
    {
        private IDistributedCache cache;

        public OrderService(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public async Task<List<Order>> GetOrders()
        {
            var key = "admin-orders";
            return await cache.GetOrCreateAsync(key, async _ => await GenerateOrders());
        }

        private async Task<List<Order>> GenerateOrders()
        {
            // In a real app, this method would async make a request to a database or API
            
            string[] names = ["Bob", "Alex", "Joe", "Jane", "Sarah", "Josh", "Ann", "Laura"];
            string[] lastNames = ["Test", "Sample", "Doe", "Example", "Testing"];
            string[] promoCodes = ["WiredFall123", "WiredCoffee", "dotnet9rocks", "Coffee123", "CoffeePromo"];
            string[] notes = ["Sample order notes", "Testing notes", "More notes", "Wired brain notes", "My notes"];
            var orders = new List<Order>();

            for (int i = 0; i < 10; i++)
            {
                var order = new Order()
                {
                    Id = i,
                    OrderNumber = new Random().Next(1, 10000),
                    Created = DateTime.Now.AddDays(new Random().Next(0, 100) * -1).AddHours(new Random().Next(0, 10) * -1),
                    FirstName = names[new Random().Next(0, names.Length)],
                    LastName = lastNames[new Random().Next(0, lastNames.Length)],
                    Notes = notes[new Random().Next(0, notes.Length)],
                    PromoCode = promoCodes[new Random().Next(0, promoCodes.Length)]
                };

                for (int y = 0; y < new Random().Next(1, 10); y++)
                {
                    order.Items.Add(new MenuItem());
                }

                orders.Add(order);
            }

            return orders;
        }

        public async Task<Order> GetOrderById(int id)
        {
            return (await GenerateOrders()).FirstOrDefault(x => x.Id == id);
        }
    }
}
