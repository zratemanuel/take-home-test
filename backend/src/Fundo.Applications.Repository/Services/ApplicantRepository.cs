using Fundo.Applications.Repository.Entity;
using Fundo.Applications.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Applications.Repository.Services
{
    public class ApplicantRepository : IApplicantRepository
    {
        protected readonly ContextDB _context;

        public ApplicantRepository(ContextDB context)
        {
            _context = context;
        }

        public async Task<Applicant?> GetApplicantAsync(string applicantId)
        {
            return await _context.Applicants.FirstOrDefaultAsync(a => a.ApplicantId == applicantId); 
        }

        public async Task<Applicant?> LoginApplicantAsync(string user, string password)
        {
            return await _context.Applicants.FirstOrDefaultAsync(a => a.User == user && a.Password == password);
        }
    }
}
