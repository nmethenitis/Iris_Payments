using IrisPayments.Data.Models;

namespace IrisPayments.Models;
public class HistoryPaymentCodeResponse {
    public bool Success { get;set; }
    public List<PaymentLogs> PaymentLogs { get; set; }
    public ErrorResponse Error { get; set; }
}
