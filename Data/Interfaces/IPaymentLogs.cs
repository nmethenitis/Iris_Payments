using IrisPayments.Data.Models;
namespace IrisPayments.Data.Interfaces;
public interface IPaymentLogs {
    Task<bool> Exists(string paymentCode);
    Task<bool> Exists(string RID, string paymentCode);
    Task<bool> ExistsPaid(string RF);
    Task<List<PaymentLogs>> GetAll(string PaymentRFID);
    Task AddAsync(PaymentLogs paymentLogs);
    void Add(PaymentLogs paymentLogs);
}
