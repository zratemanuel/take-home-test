using System.ComponentModel.DataAnnotations.Schema;

namespace Fundo.Applications.Repository.Entity;

public class HistoryDeduce
{
    [Column("HistoryDeduceId", TypeName = "varchar(36)")]
    public string? HistoryDeduceId { get; set; }
    public decimal Amount { get; set; }
    public decimal CurrentBalance { get; set; }
    public int Status { get; set; }
    public string LoanId { get; set; } = string.Empty;

    public HistoryDeduce() =>
        HistoryDeduceId = string.IsNullOrEmpty(HistoryDeduceId) ? BaseEntity.GenerateId() : HistoryDeduceId;
}
