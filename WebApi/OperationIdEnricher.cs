using Serilog.Core;
using Serilog.Events;

namespace WebApi;

public class OperationIdEnricher: ILogEventEnricher
{
    public void Enrich (LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (!logEvent.Properties.ContainsKey("Rid"))
        {
            var operationProperty = propertyFactory.CreateProperty("Rid", Guid.NewGuid().ToString());
            logEvent.AddPropertyIfAbsent(operationProperty);
        }
    }
}