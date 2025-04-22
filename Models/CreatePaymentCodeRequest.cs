using System.ComponentModel.DataAnnotations;

namespace IrisPayments.Models;
public class CreatePaymentCodeRequest {
    [Required]
    public long OrderId { get; set; }
    [Required]
    public long CustomerId { get; set; }
    [Required]
    public double Amount { get; set; }
}
