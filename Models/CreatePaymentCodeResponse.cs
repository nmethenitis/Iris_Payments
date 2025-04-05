using IrisPayments.Data.Models;

namespace IrisPayments.Models;
public class CreatePaymentCodeResponse {
    public bool Success { get; set; }
    public PaymentCode PaymentCode { get; set; }
    public ErrorResponse Error { get; set; }
}