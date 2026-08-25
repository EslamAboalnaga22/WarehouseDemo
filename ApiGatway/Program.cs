var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Yarp 
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Use(async (context, next) =>
{
    Console.WriteLine("========== REQUEST ==========");
    Console.WriteLine($"Method : {context.Request.Method}");
    Console.WriteLine($"URL    : {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");

    await next();

    Console.WriteLine("========== RESPONSE ==========");
    Console.WriteLine($"Status : {context.Response.StatusCode}");
});

app.MapReverseProxy();

app.Run();
