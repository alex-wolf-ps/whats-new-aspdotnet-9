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
builder.Services.AddOpenApi("wiredapi", options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "Wired Brain Coffee API",
            Version = "2.0.0",
            Description = "An API for the public Wired Brain Coffee site."
        };
        return Task.CompletedTask;
    });
    options.AddSchemaTransformer((schema, context, cancellationToken) =>
    {
        if (context.JsonTypeInfo.Type == typeof(decimal))
        {
            schema.Format = "decimal";
        }
        return Task.CompletedTask;
    });
});

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
})
.WithName("GetOrders")
.WithSummary("Gets the order history.")
.WithTags("order");

app.MapGet("/orders/{id}", async (IOrderService orderService, int id) =>
{
    return await orderService.GetOrderById(id);
})
.WithName("GetOrder")
.WithSummary("Gets an order by id.")
.WithTags("order");

app.MapGet("/menu", (HttpContext context, IMenuService menuService) =>
{
    return menuService.GetMenuItems();
})
.WithName("GetMenuItems")
.WithSummary("Gets the current public menu.")
.WithTags("menu");

app.Run();
