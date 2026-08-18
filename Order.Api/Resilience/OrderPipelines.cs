using Polly;

namespace Order.Api.Resilience
{
    public static class OrderPipelines
    {
        public static void Configure(ResiliencePipelineBuilder<HttpResponseMessage> builder, IHttpClientFactory httpClientFactory)
        {
            //RetryPolicy.Configure(builder);
            //RateLimiterPolicy.Configure(builder);
            HedjingPolicy.Configure(builder, httpClientFactory);
            //FallbackPolicy.Configure(builder);
            //TimeoutPolicy.Configure(builder);
            //CircuitBreakerPolicy.Configure(builder);
        }
    }
}
