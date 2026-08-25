using Order.Api.Resilience;
using Order.Api.Services;
using Warehouse.SharedLibrary.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

SharedServiceContainer.AddSharedServices(builder.Services, builder.Configuration, builder.Configuration["MySerilog:FileName"]);

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddHttpClient("OrderClient", client =>
{ 
    //client.BaseAddress = new Uri("http://localhost:5001/");
    client.BaseAddress = new Uri("http://localhost:5000/");
})
    .AddResilienceHandler("order-pipeline", (builder, context) =>
    {
        var factory = context.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        OrderPipelines.Configure(builder, factory);
    });

builder.Services.AddHttpClient("AlternativeClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5003/");
})
    .AddResilienceHandler("order-pipeline", (builder, context) =>
    {
        var factory = context.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        OrderPipelines.Configure(builder, factory);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

SharedServiceContainer.UseSharedPolicies(app);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
