using ApiGatway.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Yarp 
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Authencation
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(ClaimTypes.Role, "admin");
    });

    options.AddPolicy("UserOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("user");
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseMiddleware<AttachSignatureToRequest>();

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
