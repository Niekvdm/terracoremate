using Polly;
using Polly.Extensions.Http;

namespace TerracoreMate.Http.Policies;

public class RetryPolicy
{
    /// <summary>
    /// Gets a retry policy with the given configuration.
    /// </summary>
    /// <param name="maxAttempts">The maximum number of retry attempts.</param>
    /// <returns>A policy that handles transient HTTP errors, 'NotFound' and 'TooManyRequests' statuses and executes a retry based on the defined conditions and wait duration.</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int maxAttempts = 2)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(maxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(maxAttempts, retryAttempt)));
    }
}