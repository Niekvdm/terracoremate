using System.Reflection;
using Refit;
using TerracoreMate.Handlers.Factories;
using TerracoreMate.Handlers.Interfaces;
using TerracoreMate.Hive;
using TerracoreMate.Http.Handlers;
using TerracoreMate.Http.Policies;
using TerracoreMate.Http.Services;
using TerracoreMate.Services;

namespace TerracoreMate.Extensions;

/// <summary>
/// This static class provides extension methods for the IServiceCollection interfaces.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the require TerracoreMate services to the ServiceCollection for the DI container
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddTerracoreMate(this IServiceCollection services)
    {
        services.AddHandlers();
        services.AddRefitClients();

        services.AddSingleton<HiveSigner>();
        services.AddTransient<RetryPolicyHandler>();
        services.AddTransient<HiveHttpMessageHandler>();

        services.AddSingleton<PlayerService>();
        services.AddSingleton<TransactionService>();
        services.AddSingleton<HiveService>();

        services.AddSingleton<IHandlerFactory, HandlerFactory>();

        return services;
    }
        
    /// <summary>
    /// Adds the required refit clients for api exposure in the DI container.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the refit clients to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    private static IServiceCollection AddRefitClients(this IServiceCollection services)
    {
        services.AddRefitClient<ITerracoreService>(RefitExtensions.GetNewtonsoftJsonRefitSettings())
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var baseUrl = configuration.GetValue<string>("Terracore:ApiUrl");

                client.BaseAddress = new Uri(baseUrl);
            })
            .AddHttpMessageHandler<RetryPolicyHandler>()
            .AddPolicyHandler(RetryPolicy.GetRetryPolicy())
            .AddPolicyHandler(CircuitBreakerPolicy.GetCircuitBreakerPolicy());

        services.AddRefitClient<IHiveBroadcastService>(RefitExtensions.GetNewtonsoftJsonRefitSettings())
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var baseUrls = configuration.GetSection("Hive:Endpoints").Get<string[]>();

                client.BaseAddress = new Uri(baseUrls.First());
            })
            .AddHttpMessageHandler<HiveHttpMessageHandler>()
            .AddPolicyHandler(HiveRetryPolicy.GetRetryPolicy())
            .AddPolicyHandler(CircuitBreakerPolicy.GetCircuitBreakerPolicy());

        services.AddRefitClient<IHiveGlobalService>(RefitExtensions.GetNewtonsoftJsonRefitSettings())
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var baseUrls = configuration.GetSection("Hive:Endpoints").Get<string[]>();

                client.BaseAddress = new Uri(baseUrls.First());
            })
            .AddHttpMessageHandler<HiveHttpMessageHandler>()
            .AddPolicyHandler(HiveRetryPolicy.GetRetryPolicy())
            .AddPolicyHandler(CircuitBreakerPolicy.GetCircuitBreakerPolicy());

        return services;
    }
    
    /// <summary>
    /// Adds all handler types from the current executing assembly to the provided IServiceCollection.
    /// Types are added as transient services.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the handler types to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    private static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        // get the current assembly
        var assembly = Assembly.GetExecutingAssembly();

        // get all types that implement the IHandler interface
        var handlerTypes = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IHandler).IsAssignableFrom(t));

        // register them as transient
        foreach (var type in handlerTypes)
            services.AddTransient(type);

        return services;
    }
}