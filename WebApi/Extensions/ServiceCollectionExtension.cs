using Microsoft.Extensions.Options;

namespace WebApi.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddOption<TOption>(this IServiceCollection services, string key) where TOption : class, new()
    {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        services.Configure<TOption>(configuration.GetSection(key));
        services.AddScoped<TOption>(_ => _.GetRequiredService<IOptions<TOption>>().Value);

        return services;
    }
}