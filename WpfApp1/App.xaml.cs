using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using WpfApp1.Models;

namespace WpfApp1;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;

    public App ()
    {
        _host = new HostBuilder()
            .ConfigureAppConfiguration((hostContent, config) =>
            {
                config.AddJsonFile("appsettings.json", false, true);
            })
            .ConfigureServices((hostContent, services) =>
            {
                var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
                services.AddSingleton(_ => _.GetRequiredService<IOptions<AppSettings>>().Value);

                var appSettings = services.BuildServiceProvider().GetRequiredService<IOptions<AppSettings>>().Value;

                services.AddSingleton<MainWindow>();
                
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

                services.AddLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                    logging.AddSerilog(serilogLogger, true);
                });
            })
            .Build();
        
        using var serviceScope = _host.Services.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;
        try
        {
            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}