using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;
using TerracoreMate.Hive;
using TerracoreMate.Models.Hive;

namespace TerracoreMate.Http.Handlers;

public class HiveHttpMessageHandler : DelegatingHandler
{
    private readonly IServiceProvider _serviceProvider;

    public HiveHttpMessageHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var key = new HttpRequestOptionsKey<HiveAction>("Hive");

        if (request.Headers.TryGetValues("method", out var headerValues))
        {
            SetRequestContent(headerValues.First(), request);
        }
        else if (request.Options.TryGetValue(key, out var model))
        {
            await SetSignedTransactionRequestContent(model, request);
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private static void SetRequestContent(string headerValue, HttpRequestMessage request)
    {
        var hiveModelBody = new HiveAction(headerValue).ToBody();
        request.Content = new StringContent(JsonConvert.SerializeObject(hiveModelBody),
            Encoding.UTF8, MediaTypeNames.Application.Json);
    }

    private async Task SetSignedTransactionRequestContent(HiveAction model, HttpRequestMessage request)
    {
        var hiveApi = _serviceProvider.GetRequiredService<HiveSigner>();
        var hiveSignedTransaction = await model.ToSignedTransaction(hiveApi);
        var data = JsonConvert.SerializeObject(hiveSignedTransaction, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });

        request.Content = new StringContent(data, Encoding.UTF8, MediaTypeNames.Application.Json);
    }
}