using Fundo.Applications.Repository.Entity;

namespace Fundo.Applications.Repository.Interface
{
    public interface IApplicantRepository
    {
        Task<Applicant?> GetApplicantAsync(string applicantId);

        Task<Applicant?> LoginApplicantAsync(string user, string password);
    }
}
