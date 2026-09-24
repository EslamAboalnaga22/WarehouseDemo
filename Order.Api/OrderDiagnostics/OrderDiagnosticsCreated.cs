using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Order.Api.OrderDiagnostics
{
    public static class OrderDiagnosticsCreated
    {
        public const string ServiceName = "Order.Api";

        public static readonly ActivitySource ActivitySource = new(ServiceName);

        public static readonly Meter Meter = new(ServiceName);

        public static readonly Counter<long> OrdersCreated =
            Meter.CreateCounter<long>(
                "orders_created",
                "{order}",
                "Total number of orders created");
    }
}
