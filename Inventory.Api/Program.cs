using Inventory.Api.Consumers;
using Inventory.Api.Services;
using Warehouse.SharedLibrary.Configuration;
using Warehouse.SharedLibrary.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddOpenApi();

SharedServiceContainer.AddSharedServices(builder.Services, builder.Configuration, builder.Configuration["MySerilog:FileName"]);

builder.Services.AddScoped<IProductServices, ProductServices>();

// RabbitMQ configuration
var rabbitMqConfig = builder.Configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();
builder.Services.AddSingleton(rabbitMqConfig);
builder.Services.AddHostedService<OrderCreatedConsumer>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

SharedServiceContainer.UseSharedPolicies(app);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
