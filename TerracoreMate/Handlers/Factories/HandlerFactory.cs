using TerracoreMate.Handlers.Interfaces;
using TerracoreMate.Models;

namespace TerracoreMate.Handlers.Factories;

public interface IHandlerFactory
{
    /// <summary>
    /// Creates an instance of a handler with a specified account.
    /// </summary>
    /// <param name="account">The account to be used in created handler context.</param>
    /// <typeparam name="T">The type of the handler to be created, must implement IHandler.</typeparam>
    /// <returns>An instance of a handler.</returns>
    IHandler CreateHandler<T>(Account account);

    /// <summary>
    /// Attempts to fetch the requested service from the DI container
    /// </summary>
    /// <typeparam name="T">The type of service to be returned</typeparam>
    /// <returns>An instance of a service</returns>
    T GetService<T>();
}

public class HandlerFactory : IHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// The constructor of HandlerFactory class.
    /// </summary>
    /// <param name="serviceProvider">Service provider for accessing registered services.</param>
    public HandlerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Creates an instance of a handler with a specified account.
    /// </summary>
    /// <param name="account">The account to be used in created handler context.</param>
    /// <typeparam name="T">The type of the handler to be created, must implement IHandler.</typeparam>
    /// <returns>An instance of a handler.</returns>
    public IHandler CreateHandler<T>(Account account)
    {
        var handler = (IHandler)_serviceProvider.GetRequiredService(typeof(T));
        handler.SetAccount(account);

        return handler;
    }

    /// <summary>
    /// Attempts to fetch the requested service from the DI container
    /// </summary>
    /// <typeparam name="T">The type of service to be returned</typeparam>
    /// <returns>An instance of a service</returns>
    public T GetService<T>()
    {
        return (T)_serviceProvider.GetRequiredService(typeof(T));
    }
}