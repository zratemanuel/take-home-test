using FluentValidation.Results;
using Fundo.Applications.Domain.Models;
using Fundo.Applications.Repository.Entity;

namespace Fundo.Applications.Domain.Interfaces
{
    /// <summary>
    /// Defines operations related to loan management and processing.
    /// </summary>
    public interface ILoanManagementService
    {
        /// <summary>
        /// Registers a new loan request after validating input data.
        /// </summary>
        /// <param name="request">Loan request data.</param>
        /// <returns>A validation result indicating success or containing validation errors.</returns>
        Task<ValidationResult> InsertLoanAsync(RequestLoan request);

        /// <summary>
        /// Performs a deduction on an existing loan, such as a payment or adjustment.
        /// </summary>
        /// <param name="loanId">The identifier of the loan.</param>
        /// <param name="deductionRequest">Information about the deduction.</param>
        /// <returns>A validation result indicating whether the operation was successful.</returns>
        Task<ValidationResult> DeductLoanAsync(string loanId, RequestDeduce deductionRequest);

        /// <summary>
        /// Retrieves detailed information for a specific loan.
        /// </summary>
        /// <param name="loanId">The identifier of the loan.</param>
        /// <returns>The loan details, or null if not found.</returns>
        Task<ApplicantLoan?> GetLoanDetailsAsync(string loanId);

        /// <summary>
        /// Retrieves all existing loans in the system.
        /// </summary>
        /// <returns>A list of loans.</returns>
        Task<IReadOnlyList<ApplicantLoan>> GetAllLoansAsync();
    }
}
