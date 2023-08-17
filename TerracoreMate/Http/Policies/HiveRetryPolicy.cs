using Polly;
using Polly.Extensions.Http;

namespace TerracoreMate.Http.Policies;

public class HiveRetryPolicy
{
    /// <summary>
    /// Gets a retry policy with given configuration.
    /// </summary>
    /// <param name="maxAttempts">The maximum number of retry attempts.</param>
    /// <returns>A policy that handles transient HTTP errors and executes a retry based on the defined conditions and wait duration.</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int maxAttempts = 2)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(response => !response.IsSuccessStatusCode)
            .WaitAndRetryAsync(maxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(maxAttempts, retryAttempt)));
    }
}