using Polly;
using Polly.Timeout;

namespace Order.Api.Resilience
{
    public static class TimeoutPolicy
    {
        public static void Configure(ResiliencePipelineBuilder<HttpResponseMessage> builder)
        {
            builder.AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(5), 

                OnTimeout = args =>
                {
                    // Log the timeout event
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine($"Request timed out after {args.Timeout.TotalSeconds} seconds.");
                    Console.WriteLine("--------------------------------------------");
                    Console.ResetColor();
                    return default;
                }
            });
        }
    }
}
