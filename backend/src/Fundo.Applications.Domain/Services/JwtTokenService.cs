using Fundo.Applications.Domain.Interfaces;
using Fundo.Applications.Domain.Models;
using Fundo.Applications.Domain.Validations;
using Fundo.Applications.Repository.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fundo.Applications.Domain.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IApplicantRepository _applicantRepository;

        private const string JwtSection = "jwt";
        private const string SecretKeyName = "secretKey";
        private const string DefaultIssuer = "yourdomain.com";
        private const string DefaultAudience = "yourdomain.com";
        private const int TokenExpirationMinutes = 30;

        public JwtTokenService(IConfiguration configuration, IApplicantRepository applicantRepository)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _applicantRepository = applicantRepository ?? throw new ArgumentNullException(nameof(applicantRepository));
        }

        public async Task<string> LoginUserAsync(RequestLogin requestLogin)
        {

            var user = await _applicantRepository.LoginApplicantAsync(requestLogin.User, requestLogin.Password);

            if (user is null)
                return string.Empty;

            return GenerateJwtToken(user.User);
        }

        private string GenerateJwtToken(string username)
        {
            var secretKey = _configuration.GetSection($"{JwtSection}:{SecretKeyName}").Value;

            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException("JWT secret key is not configured.");

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, username),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: DefaultIssuer,
                audience: DefaultAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(TokenExpirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
