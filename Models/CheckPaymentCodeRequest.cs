using System.ComponentModel.DataAnnotations;

namespace IrisPayments.Models;
public class CheckPaymentCodeRequest {
    [Required]
    public string PaymentCode { get; set; }
    [Required]
    public double Amount { get; set; }
}
