using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fundo.Applications.Repository.Entity;

public class Applicant
{
    [MaxLength(36)]
    public string? ApplicantId { get; set; }

    [Column("ApplicantName", TypeName = "varchar(100)")]
    public string? ApplicantName { get; set; }

    [Column("Document", TypeName = "varchar(20)")]
    public string? Document { get; set; }

    [Column("User", TypeName = "varchar(20)")]
    public string? User { get; set; }

    [Column("Password", TypeName = "varchar(20)")]
    public string? Password { get; set; }

    public List<Loan>? Loans { get; set; }

    public Applicant() =>
        ApplicantId = string.IsNullOrEmpty(ApplicantId) ? BaseEntity.GenerateId() : ApplicantId;
}
