using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Inventory.Api.InventoryDiagnostics
{
    public static class InventoryDiagnosticsReaded
    {
        public const string ServiceName = "Inventory.Api";

        public static readonly ActivitySource ActivitySource = new(ServiceName);

        public static readonly Meter Meter = new(ServiceName);

        public static readonly Counter<long> InventoryReaded =
            Meter.CreateCounter<long>(
                "inventory_read",
                "{item}",
                "Total number of inventory items read");
    }
}
