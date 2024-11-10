using Microsoft.AspNetCore.Components;
using WiredBrainCoffee.Models;
using WiredBrainCoffee.UI.Services;

namespace WiredBrainCoffee.UI.Components.Pages
{
    public partial class Order
    {
        [Inject]
        public IMenuService MenuService { get; set; }

        [Inject]
        public NavigationManager NavManager { get; set; }

        private List<MenuItem> CurrentOrder = new List<MenuItem>();
        private List<MenuItem> MenuItems = new List<MenuItem>();
        private List<MenuItem> FilteredMenu = new List<MenuItem>();
        private decimal OrderTotal = 0m;
        private decimal SalesTax = 0.06m;
        private decimal Tip = 0m;
        private string SearchTerm = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            MenuItems = await MenuService.GetMenuItems();
        }

        private void FilterMenu()
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                FilteredMenu = MenuItems
                    .Where(x => x.Name.ToLower().Contains(SearchTerm.ToLower())).ToList();
            } 
            else
            {
                FilteredMenu = new();
            }
        }

        private void AddToOrder(MenuItem item)
        {
            CurrentOrder.Add(new MenuItem()
            {
                Name = item.Name,
                Id = item.Id,
                Price = item.Price,
            });

            OrderTotal += item.Price;
        }

        private void RemoveFromOrder(MenuItem item)
        {
            CurrentOrder.Remove(item);
            OrderTotal -= item.Price;
        }

        private void PlaceOrder()
        {
            NavManager.NavigateTo("order-confirmation");
        }
    }
}
