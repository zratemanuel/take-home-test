using Fundo.Applications.Repository.Entity;

namespace Fundo.Applications.Repository.Interface;

public interface IHistoryRepository
{

    Task InsertHistoryLoanAsync(HistoryDeduce historyDeduce);
}
