using Fundo.Applications.Repository.Entity;
using Fundo.Applications.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Applications.Repository.Services
{
    public class LoanRepository : ILoanRepository
    {
        protected readonly ContextDB _context;
        public LoanRepository(ContextDB context)
        {
            _context = context;
        }

        public async Task<ApplicantLoan?> GetLoanDetailsAsync(string loanId)
        {
            var loan = await _context.Loans!
                .Include(loan => loan.Applicant)
                .Select(loan => new ApplicantLoan
                {
                    LoanId = loan.LoanId,
                    Amount = loan.Amount,
                    CurrentBalance = loan.CurrentBalance,
                    DateInsert = loan.DateInsert,
                    DateUpdate = loan.DateUpdate,
                    Status = loan.Status,
                    ApplicantId = loan.ApplicantId,
                    ApplicantName = loan.Applicant != null ? loan.Applicant.ApplicantName! : string.Empty,
                    ApplicantDocument = loan.Applicant != null ? loan.Applicant.Document! : string.Empty
                })
                .FirstOrDefaultAsync(x => x.LoanId!.Equals(loanId));

            return loan;
        }

        public async Task<List<ApplicantLoan>> GetLoansAsync()
        {
            var loans = await _context.Loans!

            .Include(loan => loan.Applicant)
            .Select(loan => new ApplicantLoan
            {
                LoanId = loan.LoanId,
                Amount = loan.Amount,
                CurrentBalance = loan.CurrentBalance,
                Status = loan.Status,
                DateInsert = loan.DateInsert,
                DateUpdate = loan.DateUpdate,
                ApplicantId = loan.ApplicantId,
                ApplicantName = loan.Applicant!.ApplicantName!,
                ApplicantDocument = loan.Applicant.Document!
            })
           .ToListAsync();

            return loans!;
        }

        public async Task InsertLoanAsync(Loan requestLoan)
        {
            await _context.Loans!.AddAsync(requestLoan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLoanAsync(Loan requestLoan)
        {
            _context.Loans!.Update(requestLoan);

            await _context.SaveChangesAsync();
        }
    }
}
