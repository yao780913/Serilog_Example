using Serilog.Core;
using Serilog.Events;

namespace AzureTableStorage;

public class OperationEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Properties.ContainsKey("Rid")) 
            return;

        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("Rid", Guid.NewGuid().ToString()));
    }
}