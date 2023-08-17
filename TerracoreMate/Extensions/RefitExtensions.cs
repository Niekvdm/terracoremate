using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;

namespace TerracoreMate.Extensions;

/// <summary>
/// This static class provides extension methods to the Refit library.
/// </summary>
public static class RefitExtensions
{
    /// <summary>
    /// Creates an instance of type T via Refit, with a specified base URL and specified JSON serialization settings.
    /// </summary>
    /// <param name="hostUrl">The base URL for the REST services.</param>
    /// <returns>An instance of T.</returns>
    public static T For<T>(string hostUrl) => RestService.For<T>(hostUrl, GetNewtonsoftJsonRefitSettings());
    
    /// <summary>
    /// Creates an instance of type T via Refit, with a specified HttpClient and specified JSON serialization settings.
    /// </summary>
    /// <param name="client">The HttpClient for the REST services.</param>
    /// <returns>An instance of T.</returns>
    public static T For<T>(HttpClient client) => RestService.For<T>(client, GetNewtonsoftJsonRefitSettings());

    /// <summary>
    /// Provides the Newtonsoft JSON serialization settings for Refit.
    /// </summary>
    /// <returns>A RefitSettings object with Newtonsoft JSON serialization settings.</returns>
    public static RefitSettings GetNewtonsoftJsonRefitSettings() => new(new NewtonsoftJsonContentSerializer(new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
}