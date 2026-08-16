using Polly;
using Polly.Retry;
using System.Net;

namespace Order.Api.Resilience
{
    public static class RetryPolicy
    {
        public static void Configure(ResiliencePipelineBuilder<HttpResponseMessage> builder)
        {
            builder.AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(r =>
                         r.StatusCode == HttpStatusCode.RequestTimeout ||
                         r.StatusCode == HttpStatusCode.TooManyRequests ||
                         (int)r.StatusCode >= 500
                    ),

                MaxRetryAttempts = 3,

                Delay = TimeSpan.FromSeconds(2),

                BackoffType = DelayBackoffType.Exponential,

                MaxDelay = TimeSpan.FromSeconds(10),

                UseJitter = true,

                OnRetry = args =>
                {
                    // Log the retry attempt
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine($"Retrying request. Attempt: #{args.AttemptNumber + 1}, #Duration: {args.Duration}");
                    Console.WriteLine("--------------------------------------------");
                    Console.ResetColor();

                    return default;
                }
            });
        }
    }
}
