using System.ComponentModel.DataAnnotations.Schema;

namespace Fundo.Applications.Repository.Entity;

public class Loan
{
    [Column("LoanId", TypeName = "varchar(36)")]
    public string LoanId { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public decimal CurrentBalance { get; set; }

    public int Status { get; set; }

    public string ApplicantId { get; set; } = string.Empty;

    public Applicant? Applicant { get; set; }

    public DateTime DateInsert { get; set; }

    public DateTime DateUpdate { get; set; }
}
