using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using WebApi;
using WebApi.Extensions;
using WebApi.Middlewares;
using WebApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOption<AppSettings>("AppSettings");

var serilogLogger = new LoggerConfiguration()
    .Enrich.WithProperty("Oid", Guid.NewGuid().ToString())
    .Enrich.With(new OperationIdEnricher())
    .Enrich.WithExceptionDetails()
    .WriteTo.Console()
    .WriteTo.File(new JsonFormatter(), 
        path: "Logs/.log",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true,
        fileSizeLimitBytes: 1024 * 1024 * 100) // 100MB
    .CreateLogger();

builder.Services.AddLogging(logging =>
{
    logging.AddSerilog(serilogLogger, true);
});

var app = builder.Build();

app.UseMiddleware<RequestMiddleware>();
app.UseMiddleware<ResponseMiddleware>();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseSerilogRequestLogging(options =>
// {
//     options.MessageTemplate = "Handled {RequestPath}";
//     options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;
//     options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
//     {
//         diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
//         diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
//     };
// });

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/api/weatherforecast", (ILoggerFactory loggerFactory) =>
    {
        var logger = loggerFactory.CreateLogger("Program");
        logger.LogInformation("Hello world");
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateTime.Now.AddDays(index),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);
}