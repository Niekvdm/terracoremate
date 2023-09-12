using TerracoreMate.Http.Services;
using TerracoreMate.Models;
using TerracoreMate.Models.Terracore;

namespace TerracoreMate.Services;

public class TransactionService
{
    private readonly ITerracoreService _terracoreService;
    private readonly ILogger _logger;

    public TransactionService(ITerracoreService terracoreService, ILogger logger)
    {
        _terracoreService = terracoreService;
        _logger = logger;
    }

    public async Task<TransactionQueue> GetTransactionQueue()
    {
        return await _terracoreService.GetTransactionQueue();
    }
}