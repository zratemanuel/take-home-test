using FluentValidation;
using Fundo.Applications.Domain.Models;
using Fundo.Applications.Repository.Entity;
using Fundo.Applications.Repository.Interface;

namespace Fundo.Applications.Domain.Validations
{
    public class LoanValidation : AbstractValidator<RequestLoan>
    {
        private readonly IApplicantRepository _applicantRepository;

        public LoanValidation(IApplicantRepository applicantRepository)
        {
            _applicantRepository = applicantRepository ?? throw new ArgumentNullException(nameof(applicantRepository));

            RuleFor(x => x.ApplicantId)
                .NotEmpty().WithMessage("ApplicantId is required.")
                .MustAsync(ApplicantExists).WithMessage("Applicant does not exist.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Loan amount must be greater than zero.")
                .LessThanOrEqualTo(1_000_000).WithMessage("Loan amount exceeds maximum allowed (1,000,000).");

    
        }

        private async Task<bool> ApplicantExists(string applicantId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(applicantId))
                return false;

            var applicant = await _applicantRepository.GetApplicantAsync(applicantId);
            return applicant != null;
        }

    }

    public class LoanDeductValidation : AbstractValidator<RequestDeduce>
    {
        public LoanDeductValidation(ApplicantLoan foundLoan)
        {
            if (foundLoan is null)
                throw new ArgumentNullException(nameof(foundLoan));

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Deduction amount must be greater than zero.")
                .Must(amount => amount <= foundLoan.CurrentBalance)
                .WithMessage($"Deduction cannot exceed current balance ({foundLoan.CurrentBalance}).");

            RuleFor(_ => foundLoan.LoanId)
                .NotEmpty().WithMessage("Loan not found.");

            RuleFor(_ => foundLoan.Status)
                .Must(status => status != (int)Common.Enums.StatusLoan.Paid)
                .WithMessage("Cannot deduct from a fully paid loan.");
        }
    }
}
