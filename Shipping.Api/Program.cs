using Shipping.Api.Services;
using Warehouse.SharedLibrary.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

SharedServiceContainer.AddSharedServices(builder.Services, builder.Configuration, builder.Configuration["MySerilog:FileName"]);

builder.Services.AddScoped<TestHedjing>();

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
