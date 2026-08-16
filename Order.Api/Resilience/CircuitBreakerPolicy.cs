using Polly;
using Polly.CircuitBreaker;

namespace Order.Api.Resilience
{
    public static class CircuitBreakerPolicy
    {
        public static void Configure(ResiliencePipelineBuilder<HttpResponseMessage> builder)
        {
            builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(r =>
                         r.StatusCode == System.Net.HttpStatusCode.RequestTimeout ||
                         r.StatusCode == System.Net.HttpStatusCode.TooManyRequests ||
                         (int)r.StatusCode >= 500
                    ),
                FailureRatio = 0.5, 
                
                MinimumThroughput = 10, 

                SamplingDuration = TimeSpan.FromSeconds(30), 

                BreakDuration = TimeSpan.FromSeconds(10), 

                OnOpened = args =>
                {
                    // Log the circuit breaker state change
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine($"Circuit Breaker Opened.");
                    Console.WriteLine("--------------------------------------------");
                    Console.ResetColor();
                    return default;
                },
                OnHalfOpened = args =>
                {
                    // Log the circuit breaker state change
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine($"Circuit Breaker Half-Opened.");
                    Console.WriteLine("--------------------------------------------");
                    Console.ResetColor();
                    return default;
                },
                OnClosed = args =>
                {
                    // Log the circuit breaker state change
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine($"Circuit Breaker Closed.");
                    Console.WriteLine("--------------------------------------------");
                    Console.ResetColor();
                    return default;
                },
            });
        }
    }
}

