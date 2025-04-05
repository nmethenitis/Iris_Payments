using System.ComponentModel.DataAnnotations;

namespace IrisPayments.Data.Models;
public class PaymentLogs {
    [Key]
    [Required]
    [System.ComponentModel.Description("Unique identifier")]
    public Guid Id { get; set; }
    public long? OrderId { get; set; }
    public long? CustomerId { get; set; }
    public string ReferenceId { get; set; }
    public string LobId { get; set; }
    public string OrganizationId { get; set; }
    public string TransactionId { get; set; }
    public string PaymentCode { get; set; }
    public double PaidAmount { get; set; }
    public string DebtorName { get; set; }
    public string DebtorBankBIC { get; set; }
    public string PaymentDtTm { get; set; }
    public string BkDate { get; set; }
    public string RemittanceInformation { get; set; }
    public string CreationDtTm { get; set; }
    public string Status { get; set; }
    public string? RejectionCode { get; set; }
    public string? RejectionReason { get; set; }
    public bool IsPaid { get; set; }
}