using Fundo.Applications.Repository.Entity;
using Fundo.Applications.Repository.Interface;

namespace Fundo.Applications.Repository.Services;

public class HistoryRepository: IHistoryRepository
{
    protected readonly ContextDB _context;

    public  HistoryRepository(ContextDB context )
    {
        _context = context;
    }
    public async Task InsertHistoryLoanAsync(HistoryDeduce historyDeduce)
    {
        await _context.HistoryDeduces!.AddAsync(historyDeduce);
        await _context.SaveChangesAsync();
    }

}
