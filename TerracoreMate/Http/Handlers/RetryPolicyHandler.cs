using System.Net;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using TerracoreMate.Http.Policies;

namespace TerracoreMate.Http.Handlers;

public class RetryPolicyHandler : DelegatingHandler
{
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;

    public RetryPolicyHandler(int maxAttempts = 2)
    {
        _retryPolicy = RetryPolicy.GetRetryPolicy(maxAttempts);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await _retryPolicy.ExecuteAsync(async () =>
        {
            response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                // Wait for a specific duration before retrying
                var retryAfter = response.Headers.RetryAfter;

                if (retryAfter != null && retryAfter.Delta.HasValue)
                {
                    await Task.Delay(retryAfter.Delta.Value);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(5)); // Default wait time
                }

                response.Dispose(); // Dispose the response to prevent resource leakage
                throw new HttpRequestException(); // Throw exception to trigger the retry
            }

            return response;
        });

        return response;
    }
}
