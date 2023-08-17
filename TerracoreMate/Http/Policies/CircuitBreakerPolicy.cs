using Polly;
using Polly.Extensions.Http;

namespace TerracoreMate.Http.Policies;

public class CircuitBreakerPolicy
{
    /// <summary>
    /// Gets an instance of Circuit Breaker policy with the provided configuration.
    /// </summary>
    /// <param name="maxEventsAllowed">The maximum number of failed requests allowed before opening the circuit.</param>
    /// <param name="breakDurationSeconds">The duration in seconds for which the circuit should stay opened before allowing a next trial request.</param>
    /// <returns>An instance of IAsyncPolicy for HttpResponseMessage.</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int maxEventsAllowed = 3, int breakDurationSeconds = 30)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(maxEventsAllowed, TimeSpan.FromSeconds(breakDurationSeconds));
    }
}