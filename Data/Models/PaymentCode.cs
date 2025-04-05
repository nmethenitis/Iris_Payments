using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IrisPayments.Data.Models;
public class PaymentCode {
    [Key]
    [Required]
    [System.ComponentModel.Description("Unique identifier")]
    public Guid Id { get; set; }
    public string Code { get; set; }
    public long? OrderID { get; set; }
    public long CustomerID { get; set; }
    public double? OrderAmount { get; set; }
    public string? IPAddress { get; set; }
    [Column(TypeName = "DateTime")]
    public DateTime CreatedAt { get; set; }
    [Column(TypeName = "DateTime")]
    public DateTime? ChangedAt { get; set; }
}