using Fundo.Applications.Domain.Models;

namespace Fundo.Applications.Domain.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> LoginUserAsync(RequestLogin requestLogin);
    }
}
