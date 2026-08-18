using Polly;
using Polly.RateLimiting;
using System.Threading.RateLimiting;

namespace Order.Api.Resilience
{
    public class RateLimiterPolicy
    {
        public static void Configure(ResiliencePipelineBuilder<HttpResponseMessage> builder)
        {
            builder.AddRateLimiter(new RateLimiterStrategyOptions
            {
                DefaultRateLimiterOptions = new ConcurrencyLimiterOptions
                {
                    PermitLimit = 1, 
                    QueueLimit = 2, 
                },

                OnRejected = args =>
                {
                    // Log the rate limit rejection
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine($"Request rejected due to rate limiting.");
                    Console.WriteLine("--------------------------------------------");
                    Console.ResetColor();
                    return default;
                }
            });
        }
    }
}
