using Polly;
using Polly.Hedging;
using Polly.Timeout;
using System.Net;

namespace Order.Api.Resilience
{
    public class HedjingPolicy
    {
        public static void Configure(ResiliencePipelineBuilder<HttpResponseMessage> builder, IHttpClientFactory httpClientFactory)
        {

            builder.AddHedging(new HedgingStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<TimeoutRejectedException>()
                    .Handle<HttpRequestException>()
                    .HandleResult(r =>
                         r.StatusCode == HttpStatusCode.RequestTimeout ||
                         r.StatusCode == HttpStatusCode.TooManyRequests ||
                         (int)r.StatusCode >= 500
                    ),

                MaxHedgedAttempts = 1,

                Delay = TimeSpan.FromSeconds(5),

                ActionGenerator = args =>
                {
                    var shippingClient =
                       httpClientFactory.CreateClient("AlternativeClient");

                    return async () =>
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("--------------------------------------------");
                        Console.WriteLine(
                            "[Hedging] Alternative attempt -> Shipping.API");
                        Console.WriteLine("--------------------------------------------");
                        Console.ResetColor();
                        

                        try
                        {
                            var response = await shippingClient.GetAsync($"api/shipping");

                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("--------------------------------------------");
                            Console.WriteLine(
                               $"[Hedging] Shipping.API returned: {(int)response.StatusCode} items");
                            Console.WriteLine("--------------------------------------------");
                            Console.ResetColor();

                           

                            return Outcome.FromResult(response);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                $"[Hedging] Shipping.API failed: {ex.Message}");

                            return Outcome.FromException<HttpResponseMessage>(ex);
                        }
                    };

                }
            });
        }
    }
}
