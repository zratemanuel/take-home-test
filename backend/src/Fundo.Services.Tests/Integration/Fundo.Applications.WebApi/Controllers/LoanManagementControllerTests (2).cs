using Fundo.Applications.Domain.Models;
using Fundo.Applications.Repository;
using Fundo.Applications.Repository.Entity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Integration
{ 
    public class LoanManagementControllerTests :
        IClassFixture<WebApplicationFactory<Fundo.Applications.WebApi.Startup>>,
        IClassFixture<WebApplicationFactory<Fundo.Applications.WebApiSecurity.Startup>>
    {
        private readonly HttpClient _authClient;
        private readonly HttpClient _clientLoan;
        private readonly IServiceProvider _serviceProvider;

        public LoanManagementControllerTests(
         WebApplicationFactory<Fundo.Applications.WebApi.Startup> factoryLoan,
         WebApplicationFactory<Fundo.Applications.WebApiSecurity.Startup> factoryAuth)
        { 
            var authFactory = factoryAuth.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                { 
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ContextDB>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }
                     
                    services.AddDbContext<ContextDB>(options =>
                        options.UseInMemoryDatabase("TestDatabase"));
                });
            });

            _authClient = authFactory.CreateClient();
             
            var loanFactory = factoryLoan.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                { 
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ContextDB>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }
                     
                    services.AddDbContext<ContextDB>(options =>
                        options.UseInMemoryDatabase("TestDatabase"));
                });
            });

            _clientLoan = loanFactory.CreateClient();
            _serviceProvider = loanFactory.Services;

            #region Inicialize in memory database

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ContextDB>();
                context.Applicants.Add(new Applicant
                {
                    ApplicantId = "12345",
                    ApplicantName = "John Doe",
                    Document = "123456789",
                    User = "john@test.com",
                    Password = "1234"
                });
                context.SaveChangesAsync();

                context.Loans.Add(new Loan
                {
                    ApplicantId = "12345",
                    Amount = 1000,
                    CurrentBalance = 1000,
                    DateInsert = DateTime.UtcNow,
                    DateUpdate = DateTime.UtcNow,
                    LoanId = "12345",

                });
                context.SaveChangesAsync();
            }

            #endregion
        }

        private async Task<string> AuthenticateAsync()
        {
            // Arrange: 
            var loginRequest = new RequestLogin
            {
                User = "john@test.com",
                Password = "1234"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginRequest),
                Encoding.UTF8,
                "application/json"
            );

            // Act: 
            var response = await _authClient.PostAsync("/api/security/login", content);

            // Assert: 
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<ResponseLogin>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return responseObject.Token;
        }

        #region PostLoanAsync

        [Fact]
        public async Task PostLoanAsync_ShouldReturn200_WhenRequestIsValid()
        {
            // Arrange:
            var token = await AuthenticateAsync();
            _clientLoan.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var requestLoan = new
            {
                Amount = 1000.0,
                Term = 12,
                ApplicantId = "12345"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestLoan),
                Encoding.UTF8,
                "application/json"
            );

            // Act: 
            var response = await _clientLoan.PostAsync("/api/loan", content);

            // Assert:
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Created successfully", responseContent);
        }         

        [Fact]
        public async Task PostLoanAsync_ShouldReturn400_WhenRequestIsNotValid()
        {
            // Arrange:
            var token = await AuthenticateAsync();
            _clientLoan.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var requestLoan = new
            {
                Amount = 1000.0,
                Term = 12 
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestLoan),
                Encoding.UTF8,
                "application/json"
            );

            // Act: 
            var response = await _clientLoan.PostAsync("/api/loan", content);

            // Assert:
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Applicant not Exists", responseContent);
        }

        [Fact]
        public async Task PostLoanAsync_ShouldReturn401_WhenNotAuthenticated()
        {
            // Arrange: 
            var content = new StringContent(
                JsonSerializer.Serialize(new RequestLoan()),
                Encoding.UTF8,
                "application/json"
            );

            // Act: 
            var response = await _clientLoan.PostAsync("/api/loan", content);

            // Assert:
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion

        #region GetLoansAsync

        [Fact]
        public async Task GetLoansAsync_ShouldReturn200_WhenLoansExist()
        {
            // Arrange:
            var token = await AuthenticateAsync();

            _clientLoan.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act: 
            var response = await _clientLoan.GetAsync("/api/loan/loans");

            // Assert:
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetLoansAsync_ShouldReturn401_WhenNotAuthenticated()
        {
            // Act: 
            var response = await _clientLoan.GetAsync("/api/loan/loans");

            // Assert:
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion
    }
}
