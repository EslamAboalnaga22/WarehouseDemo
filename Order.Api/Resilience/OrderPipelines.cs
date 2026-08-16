using Polly;

namespace Order.Api.Resilience
{
    public static class OrderPipelines
    {
        public static void Configure(ResiliencePipelineBuilder<HttpResponseMessage> builder)
        {
            RetryPolicy.Configure(builder);
            TimeoutPolicy.Configure(builder);
            CircuitBreakerPolicy.Configure(builder);
        }
    }
}
