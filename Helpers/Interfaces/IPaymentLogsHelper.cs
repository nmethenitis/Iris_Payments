using IrisPayments.Data.Models;
using IrisPayments.Models;

namespace IrisPayments.Helpers.Interfaces;
public interface IPaymentLogsHelper {
    Task<HistoryPaymentCodeResponse> GetLogHistory(string code);
    Task<PaymentLogs> CreateLog(PaymentCode code, Incomingpayments incomingPayment);
    Task<PaymentLogs> UpdateLog(PaymentLogs log, bool isPaid);
    Task<PaymentLogs> UpdateLog(PaymentLogs log, PaymentResponse paymentResponse, string rejectionReason);
    Task<PaymentLogs> UpdateLog(PaymentLogs log, IncomingPaymentsStatus incomingPaymentsStatus, string rejectionReason, bool isPaid = false);
}