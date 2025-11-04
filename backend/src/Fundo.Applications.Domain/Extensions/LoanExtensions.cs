using Fundo.Applications.Repository.Entity;

namespace Fundo.Applications.Domain.Extentions;

public static class LoanExtensions
{
    public static Loan ToEntityLoan(this ApplicantLoan applicantLoan)
    { 
        return new Loan
        {
            LoanId = applicantLoan.LoanId,
            ApplicantId = applicantLoan.ApplicantId,
            Amount = applicantLoan.Amount,
            CurrentBalance = applicantLoan.CurrentBalance,           
            DateInsert = applicantLoan.DateInsert
        };
    }
}
