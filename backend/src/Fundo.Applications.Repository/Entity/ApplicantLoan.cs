namespace Fundo.Applications.Repository.Entity
{
    public class ApplicantLoan
    {
        public string LoanId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal CurrentBalance { get; set; }
        public int Status { get; set; } = 0;
        public DateTime DateInsert { get; set; }
        public DateTime? DateUpdate { get; set; }
        public string ApplicantId { get; set; } = string.Empty;
        public string ApplicantName { get; set; } = string.Empty;
        public string ApplicantDocument { get; set; } = string.Empty;
    }
}
