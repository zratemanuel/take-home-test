using Fundo.Applications.Domain.Common.Enums;

namespace Fundo.Applications.Domain.Models;

public class RequestLoan
{
    public string LoanId { get; set; } = string.Empty;
    public string ApplicantId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal CurrentBalance { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public StatusLoan Status { get; set; } = StatusLoan.Active;
    public DateTime DateInsert { get; set; }
    public DateTime DateUpdate { get; set; }


    public Repository.Entity.Loan ToEntity()
    {
        var newLoan = new Repository.Entity.Loan();
        newLoan.LoanId = LoanId;
        newLoan.Amount = Amount;
        newLoan.CurrentBalance = CurrentBalance;
        newLoan.Status = (int)Status;
        newLoan.DateInsert = DateInsert;
        newLoan.DateUpdate = DateUpdate;
        newLoan.ApplicantId = ApplicantId;

        return newLoan;
    }
}
