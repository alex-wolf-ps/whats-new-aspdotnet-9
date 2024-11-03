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
        IDistributedCache cache { get; }

        public OrderService(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public async Task<List<Order>> GetOrders()
        {
            var key = "orders";
            var cachedOrders = await cache.GetAsync(key);

            if (cachedOrders is null)
            {
                // Orders not cached - get them from the "database"
                var orders = GenerateOrders();

                // Serialize and cache the orders
                var serializedOrders = JsonSerializer.Serialize(orders);
                cachedOrders = Encoding.UTF8.GetBytes(serializedOrders);
                await cache.SetAsync(key, cachedOrders, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
                });

                return orders;
            }
            else
            {
                // Orders found in cache - deserialize and return them
                return JsonSerializer.Deserialize<List<Order>>(cachedOrders);
            }
        }

        public Order GetOrderById(int id)
        {
            return GenerateOrders().ToList().FirstOrDefault(x => x.Id == id);
        }

        private List<Order> GenerateOrders()
        {
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
    }
}
