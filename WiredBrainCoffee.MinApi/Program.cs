using WiredBrainCoffee.MinApi.Services;
using WiredBrainCoffee.MinApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapGet("/orders", async (IOrderService orderService) =>
{
    return await orderService.GetOrders();
})
.WithName("GetOrders")
.WithSummary("Gets the order history.")
.WithTags("order")
.WithOpenApi();

app.MapGet("/orders/{id}", async (IOrderService orderService, int id) =>
{
    return await orderService.GetOrderById(id);
})
.WithName("GetOrder")
.WithSummary("Gets an order by id.")
.WithTags("order")
.WithOpenApi();

app.MapGet("/menu", (HttpContext context, IMenuService menuService) =>
{
    return menuService.GetMenuItems();
})
.WithName("GetMenuItems")
.WithSummary("Gets the current public menu.")
.WithTags("menu")
.WithOpenApi();

app.Run();
