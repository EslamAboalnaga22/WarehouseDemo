using Order.Api.Resilience;
using Order.Api.Services;
using Warehouse.SharedLibrary.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

SharedServiceContainer.AddSharedServices(builder.Services, builder.Configuration, builder.Configuration["MySerilog:FileName"]);


builder.Services.AddHttpClient<IOrderService, OrderService>(client =>
{ 
    client.BaseAddress = new Uri("http://localhost:5001/");
})
    .AddResilienceHandler("order-pipeline", OrderPipelines.Configure);

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
