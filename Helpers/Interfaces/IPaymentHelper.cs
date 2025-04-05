using IrisPayments.Data.Models;
using IrisPayments.Models;

namespace IrisPayments.Helpers.Interfaces;
public interface IPaymentHelper {
    bool Check(string code, string amount);
    bool Check(PaymentCode code, string amount);
    bool Check(PaymentCode code, double amount);
    PaymentResponse InitializeResponse(PaymentRequest paymentRequest);
    IncomingPaymentsStatus CreatePaymentResponse(string RID, bool accept, string rejectionCode = null);
    Task SendNotification(PaymentLogs paymentLogs, PaymentCode paymentIdentity);
    Task<IncomingPaymentsStatus> GetPaymentResponse(Incomingpayments incomingPayment);
}