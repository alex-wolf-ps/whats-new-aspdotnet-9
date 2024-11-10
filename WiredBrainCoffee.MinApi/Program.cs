using Microsoft.Extensions.Caching.Hybrid;
using WiredBrainCoffee.MinApi.Services;
using WiredBrainCoffee.MinApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IMenuService, MenuService>();

builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromSeconds(15),
        LocalCacheExpiration = TimeSpan.FromSeconds(15)
    };
});

builder.Services.AddCors();

builder.Services.AddOpenApi("wiredapi");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    //app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/wiredapi.json", "wiredapi"));
}

app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapGet("/orders", async (IOrderService orderService) =>
{
    return await orderService.GetOrders();
});

app.MapGet("/orders/{id}", async (IOrderService orderService, int id) =>
{
    return await orderService.GetOrderById(id);
});

app.MapGet("/menu", (HttpContext context, IMenuService menuService) =>
{
    return menuService.GetMenuItems();
})
.WithName("GetsMenuItems")
.WithSummary("Gets the current list of menu items.")
.WithTags("menu", "products");

app.Run();
