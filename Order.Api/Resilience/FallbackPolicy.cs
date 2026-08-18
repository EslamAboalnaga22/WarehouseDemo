using Order.Api.Requests_Responses;
using Polly;
using Polly.Fallback;
using Polly.Timeout;
using System.Net;

namespace Order.Api.Resilience
{
    public class FallbackPolicy
    {
        public static void Configure(ResiliencePipelineBuilder<HttpResponseMessage> builder)
        {
            builder.AddFallback(new FallbackStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<TimeoutRejectedException>()
                    .Handle<HttpRequestException>()
                    .HandleResult(r =>
                         r.StatusCode == HttpStatusCode.RequestTimeout ||
                         r.StatusCode == HttpStatusCode.TooManyRequests ||
                         (int)r.StatusCode >= 500
                    ),

                FallbackAction = context =>
                {
                    // Log the fallback event
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine("Fallback action executed due to a failure.");
                    Console.WriteLine("--------------------------------------------");
                    Console.ResetColor();

                    IEnumerable<ProductResponse> response = [
                       new ProductResponse{Id = 1,
                            Name = "Fallback Product 1",
                            Price = 9.99m,
                            Stock = 1
                        },
                        new ProductResponse{
                            Id = 2,
                            Name = "Fallback Product 2",
                            Price = 19.99m,
                            Stock = 1
                        }
                    ];

                    var fallbackResponse = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = JsonContent.Create(response)
                    };

                    return Outcome.FromResultAsValueTask(fallbackResponse);
                }
            });
        }
    }
}
