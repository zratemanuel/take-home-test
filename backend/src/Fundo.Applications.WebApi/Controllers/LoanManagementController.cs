using FluentValidation.Results;
using Fundo.Applications.Domain.Interfaces;
using Fundo.Applications.Domain.Models;
using Fundo.Applications.Repository.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fundo.Applications.WebApi.Controllers
{
    [Route("api/loan")]
    public class LoanManagementController : Controller
    {
        private readonly ILogger<LoanManagementController> _logger;
        private readonly ILoanManagementService _loanManagementService;

        public LoanManagementController(ILoanManagementService loanManagementService, ILogger<LoanManagementController> logger)
        {
            _loanManagementService = loanManagementService;
            _logger = logger;
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<ActionResult<string>> PostLoanAsync([FromBody] RequestLoan requestLoan)
        {
            try
            {
                ValidationResult result = await _loanManagementService.InsertLoanAsync(requestLoan);

                if (!result.IsValid)
                    return BadRequest(String.Join(", ", result.Errors.Select(x => x.ErrorMessage).ToList()));

                return Ok("Created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPost("{loanId}/payment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<ActionResult<string>> PaymentLoanAsync([FromRoute] string loanId, [FromBody] RequestDeduce deduceLoan)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loanId))                
                    return BadRequest("The loanId parameter is required.");
                 
                ValidationResult result = await _loanManagementService.DeductLoanAsync(loanId, deduceLoan);

                if (!result.IsValid)
                    return BadRequest(String.Join(", ", result.Errors.Select(x => x.ErrorMessage).ToList()));

                return Ok("Payment successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpGet("{loanId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<ActionResult<ApplicantLoan>> GetLoanByIdAsync([FromRoute] string loanId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loanId))
                    return BadRequest("The loanId parameter is required.");

                var loan = await _loanManagementService.GetLoanDetailsAsync(loanId);

                return loan != null ?
                    Ok(loan) :
                    StatusCode(StatusCodes.Status404NotFound, "Loan not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpGet("loans")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<ActionResult<List<ApplicantLoan>>> GetLoansAsync()
        {
            try
            {
                var loan = await _loanManagementService.GetAllLoansAsync();

                return loan.Any() ?
                    Ok(loan) :
                    StatusCode(StatusCodes.Status404NotFound, "Loans not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }
    }
}