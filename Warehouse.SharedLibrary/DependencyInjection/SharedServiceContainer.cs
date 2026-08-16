using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using Serilog;
using Microsoft.AspNetCore.Builder;
using System.Runtime.CompilerServices;
using Warehouse.SharedLibrary.MiddleWare;

namespace Warehouse.SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration, string FileName) //where TContext : DbContext
        {
            // Add Genric Database Context
            //services.AddDbContext<TContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection") , sqlserverOpt => sqlserverOpt.EnableRetryOnFailure()));

            // Configure Serilog Logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{FileName}-.text",
                        restrictedToMinimumLevel: LogEventLevel.Information,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                        rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Add JWT Authentication Scheme
            JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, configuration);

            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/openapi/v1.json", "API v1");
            });

            app.UseMiddleware<GlobalException>();

            // app.UseMiddleware<ListenOnlyToApiGateway>();

            return app;
        }
    }
}
