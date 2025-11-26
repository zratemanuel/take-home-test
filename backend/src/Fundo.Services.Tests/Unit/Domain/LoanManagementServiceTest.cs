using FluentAssertions;
using FluentValidation.Results;
using Fundo.Applications.Domain.Models;
using Fundo.Applications.Domain.Services;
using Fundo.Applications.Domain.Validations;
using Fundo.Applications.Repository.Entity;
using Fundo.Applications.Repository.Interface;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Unit.Domain;

public class LoanManagementServiceTest
{
    private readonly Mock<ILoanRepository> _loanRepositoryMock;
    private readonly Mock<IApplicantRepository> _applicantRepositoryMock;
    private readonly Mock<IHistoryRepository> _historyRepository;
    private readonly LoanManagementService _loanManagementService;

    public LoanManagementServiceTest()
    {
        _loanRepositoryMock = new Mock<ILoanRepository>();
        _applicantRepositoryMock = new Mock<IApplicantRepository>();
        _historyRepository = new Mock<IHistoryRepository>();
        _loanManagementService = new LoanManagementService(_loanRepositoryMock.Object, _applicantRepositoryMock.Object, _historyRepository.Object);
    }

    [Fact]
    public async Task InsertLoanAsync_ShouldReturnValidationResult_WhenValidationFails()
    {
        // Arrange
        var requestLoan = new RequestLoan();
        var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Field", "Validation failed")
            });

        _applicantRepositoryMock.Setup(repo => repo.GetApplicantAsync(It.IsAny<string>()))
              .ReturnsAsync((Applicant)null);

        var loanValidationMock = new Mock<LoanValidation>(_applicantRepositoryMock.Object);

        // Act
        var result = await _loanManagementService.InsertLoanAsync(requestLoan);

        // Assert
        Assert.False(result.IsValid);
        _loanRepositoryMock.Verify(repo => repo.InsertLoanAsync(It.IsAny<Loan>()), Times.Never);
    }

    [Fact]
    public async Task InsertLoanAsync_ShouldInsertLoan_WhenValidationPasses()
    {
        // Arrange
        var requestLoan = new RequestLoan()
        { 
            ApplicantId = BaseEntity.GenerateId(),
            Amount =100
        };
        
        var validationResult = new ValidationResult();

        _applicantRepositoryMock.Setup(repo => repo.GetApplicantAsync(It.IsAny<string>()))
              .ReturnsAsync(new Applicant()
              {
                  ApplicantId = BaseEntity.GenerateId(),
                  ApplicantName = "Teste",
                  Document="1234"
              }); 
        
        var loanValidationMock = new Mock<LoanValidation>(_applicantRepositoryMock.Object);
         
        _loanRepositoryMock.Setup(repo => repo.InsertLoanAsync(It.IsAny<Loan>())).Returns(Task.CompletedTask);

        // Act
        var result = await _loanManagementService.InsertLoanAsync(requestLoan);

        // Assert
        result.IsValid.Should().BeTrue();
        _loanRepositoryMock.Verify(repo => repo.InsertLoanAsync(It.IsAny<Loan>()), Times.Once);
    }

    [Fact]
    public async Task DeductLoanAsync_ShouldReturnValidationResult_WhenValidationFails()
    {
        // Arrange
        var loanId = "loan123";
        var requestDeduce = new RequestDeduce { Amount = 0 };
        var foundLoan = new ApplicantLoan { CurrentBalance = 200 };
        var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Field", "Validation failed")
            });

        _loanRepositoryMock.Setup(repo => repo.GetLoanDetailsAsync(loanId)).ReturnsAsync(foundLoan);

        var loanDeductValidationMock = new Mock<LoanDeductValidation>(foundLoan);
          
        // Act
        var result = await _loanManagementService.DeductLoanAsync(loanId, requestDeduce);

        // Assert
        Assert.False(result.IsValid);
        _loanRepositoryMock.Verify(repo => repo.UpdateLoanAsync(It.IsAny<Loan>()), Times.Never);
    }

    [Fact]
    public async Task DeductLoanAsync_ShouldUpdateLoan_WhenValidationPasses()
    {
        // Arrange
        var loanId = "loan123";
        var requestDeduce = new RequestDeduce { Amount = 100 };
        var foundLoan = new ApplicantLoan { CurrentBalance = 200,LoanId = loanId };
        var validationResult = new ValidationResult();

        _loanRepositoryMock.Setup(repo => repo.GetLoanDetailsAsync(loanId)).ReturnsAsync(foundLoan);
        _loanRepositoryMock.Setup(repo => repo.UpdateLoanAsync(It.IsAny<Loan>())).Returns(Task.CompletedTask);

        var loanDeductValidationMock = new Mock<LoanDeductValidation>(foundLoan); 

        // Act
        var result = await _loanManagementService.DeductLoanAsync(loanId, requestDeduce);

        // Assert
        result.IsValid.Should().BeTrue();
        _loanRepositoryMock.Verify(repo => repo.UpdateLoanAsync(It.IsAny<Loan>()), Times.Once);
    }

    [Fact]
    public async Task GetLoanDetailsAsync_ShouldReturnLoanDetails_WhenLoanExists()
    {
        // Arrange
        var loanId = "loan123";
        var expectedLoan = new ApplicantLoan { LoanId = loanId };

        _loanRepositoryMock.Setup(repo => repo.GetLoanDetailsAsync(loanId)).ReturnsAsync(expectedLoan);

        // Act
        var result = await _loanManagementService.GetLoanDetailsAsync(loanId);

        // Assert
        result.Should().Be(expectedLoan);
        _loanRepositoryMock.Verify(repo => repo.GetLoanDetailsAsync(loanId), Times.Once);
    }

    [Fact]
    public async Task GetLoansAsync_ShouldReturnListOfLoans()
    {
        // Arrange
        var expectedLoans = new List<ApplicantLoan>
            {
                new ApplicantLoan { LoanId = "loan1" },
                new ApplicantLoan { LoanId = "loan2" }
            };

        _loanRepositoryMock.Setup(repo => repo.GetLoansAsync()).ReturnsAsync(expectedLoans);

        // Act
        var result = await _loanManagementService.GetAllLoansAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedLoans);
        _loanRepositoryMock.Verify(repo => repo.GetLoansAsync(), Times.Once);
    }
}