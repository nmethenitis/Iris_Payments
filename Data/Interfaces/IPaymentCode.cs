using IrisPayments.Data.Models;

namespace IrisPayments.Data.Interfaces;
public interface IPaymentCode {
    Task<Guid> Add(PaymentCode paymentIdentity);
    Task<PaymentCode> Get(Guid? id);
    Task<PaymentCode> Get(string Code);
    Task<PaymentCode> GetByOrderID(long? OrderID);
    Task<PaymentCode> GetByCustomerID(long? customerID);
    Task<List<PaymentCode>> GetAll();
    Task<bool> Update(PaymentCode paymentCode);
    Task<bool> Delete(Guid? id);
}
