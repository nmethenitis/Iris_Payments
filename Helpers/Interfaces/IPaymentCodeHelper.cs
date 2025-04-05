using IrisPayments.Data.Models;
using IrisPayments.Models;

namespace IrisPayments.Helpers.Interfaces;
public interface IPaymentCodeHelper {
    Task<CheckPaymentCodeResponse> GetPaymentCode(string code);
    Task<CheckPaymentCodeResponse> CheckPaymentCode(CheckPaymentCodeRequest request);
    Task<PaymentResponse> PaymentRequest(PaymentRequest request);
    string CreatePaymentCode(long customerId, double? amount);
    Task<CreatePaymentCodeResponse> Upsert(CreatePaymentCodeRequest request);

}