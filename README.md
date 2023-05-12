# Serilog_Example

## Packages
```
install-package Microsoft.EntityFrameworkCore -version 6.0.8
install-package Microsoft.Extensions.Configuration -version 6.0.1
install-package Microsoft.Extensions.Hosting -version 6.0.1
install-package Microsoft.Extensions.Hosting.Abstractions -version 6.0.0
install-package Serilog -version 2.11.0
install-package Serilog.AspNetCore -version 6.0.1
install-package Serilog.Exceptions -version 8.4.0
install-package Serilog.Formatting.Compact -version 1.1.0
install-package Serilog.Settings.Configuration -version 3.3.0
install-package Serilog.Sinks.File -version 5.0.0
install-package Serilog.Sinks.Literate -version 3.0.0
```
### AzureTableStorage
示範 Sinks 的用法 `Serilog.Sinks.AzureTableStorage`

### WebApi
示範 `Serilog.Settings.Configuration`
如何透過 configuration 載入`serilog`的設定
* appsettings.json  
    `Write.To.File()`
* appsettings-2.json  
    `Write.To.File()`  
    `Write.To.AzureTableStorage()`

### OperationEnricher
```csharp
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
```
特別介紹這個用法，我建立一個屬性 Key = Rid  
在每一次的 Request 進來時，都會產生一個 `Guid`  
這樣就可以在 Log 中，透過 `Rid` 追蹤到這一次從 Request 到 Response 所有相關的 Log