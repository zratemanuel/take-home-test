using Fundo.Applications.Repository.Entity;

namespace Fundo.Applications.Repository.Interface
{
    public interface ILoanRepository
    {
        Task InsertLoanAsync(Loan requestLoan);
            
        Task UpdateLoanAsync(Loan requestLoan);

        Task<ApplicantLoan?> GetLoanDetailsAsync(string loanId);

        Task<List<ApplicantLoan>> GetLoansAsync();
    }
}
