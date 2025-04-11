using Azure.Core;
using IrisPayments.Data.Interfaces;
using IrisPayments.Data.Models;
using IrisPayments.Helpers.Interfaces;
using IrisPayments.Models;
using Microsoft.Identity.Client;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace IrisPayments.Helpers.Services;
public class PaymentCodeHelperService : IPaymentCodeHelper {

    private readonly IPaymentCode _paymentCodeService;
    private readonly IPaymentHelper _paymentHelperService;
    private readonly ILogger<PaymentCodeHelperService> _logger;
    private readonly IConfiguration _config;
    public PaymentCodeHelperService(IPaymentCode paymentCodeService, IPaymentHelper paymentHelper, ILogger<PaymentCodeHelperService> logger, IConfiguration config) {
        _paymentCodeService = paymentCodeService;
        _paymentHelperService = paymentHelper;
        _logger = logger;
        _config = config;
    }

    public async Task<CheckPaymentCodeResponse> GetPaymentCode(string code) {
        try {
            var paymentCode = await _paymentCodeService.Get(code);
            if (paymentCode != null) {
                return new CheckPaymentCodeResponse() {
                    Success = true,
                    PaymentCode = paymentCode
                };
            } else {
                return null;
            }
        } catch (Exception ex) {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<CheckPaymentCodeResponse> CheckPaymentCode(CheckPaymentCodeRequest request) {
        try {
            var paymentCode = await _paymentCodeService.Get(request.PaymentCode);
            if (paymentCode != null) {
                if (_paymentHelperService.Check(paymentCode, request.Amount)) {
                    return new CheckPaymentCodeResponse() {
                        Success = true,
                        PaymentCode = paymentCode
                    };
                } else {
                    return new CheckPaymentCodeResponse() {
                        Success = false,
                        Error = new ErrorResponse() {
                            ErrorCode = 2,
                            ErrorMessage = "Not the same order amount"
                        }
                    };
                }
            } else {
                return null;
            }
        } catch (Exception ex) {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<CreatePaymentCodeResponse> Upsert(CreatePaymentCodeRequest request) {
        PaymentCode paymentCode = await _paymentCodeService.GetByOrderID(request.OrderId);
        try {
            bool newPaymentID = false;
            if (paymentCode == null) {
                paymentCode = new PaymentCode();
                paymentCode.OrderID = request.OrderId;
                paymentCode.CustomerID = request.CustomerId;
                newPaymentID = true;
            }
            paymentCode.OrderAmount = request.Amount;
            paymentCode.Code = CreatePaymentCode(paymentCode.CustomerID, paymentCode.OrderAmount);
            if (newPaymentID) {
                paymentCode.CreatedAt = DateTime.Now;
                await _paymentCodeService.Add(paymentCode);
            } else {
                paymentCode.ChangedAt = DateTime.Now;
                await _paymentCodeService.Update(paymentCode);
            }
        } catch (Exception ex) {
            _logger.LogError(ex, ex.Message);
            throw;
        }
        return new CreatePaymentCodeResponse() {
            Success = true,
            PaymentCode = paymentCode
        };
    }

    public async Task<PaymentResponse> PaymentRequest(PaymentRequest request) {
        var paymentResponse = _paymentHelperService.InitializeResponse(request);
        try {
            int i = 0;
            foreach (Incomingpayments incomingPayment in request.IncomingPayments) {
                string orgNumber = incomingPayment.ActorID;
                paymentResponse.incomingPaymentsStatus[i] = await _paymentHelperService.GetPaymentResponse(incomingPayment);
                i++;
            }
        } catch (Exception ex) {
            _logger.LogError(ex, ex.Message);
            throw;
        }
        return paymentResponse;
    }

    public string CreatePaymentCode(long customerId, double? amount) {
        string orgNumber = _config.GetValue<string>("AppSettings:Dias:OrgNumber");
        try {
            var sb = new StringBuilder(orgNumber);
            sb.Append(GetTenthDigit(amount));
            string customerID = customerId.ToString("D8");
            int zeroPaddingNumber = 15 - customerID.Length;
            if(zeroPaddingNumber > 0) {
                string unixTimestampLastDigits = ((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString().Substring(0, zeroPaddingNumber);
                sb.Append(unixTimestampLastDigits);
            }
            sb.Append(customerID);
            string outputString = sb.ToString();
            string digits3to4 = GetDigits3to4(outputString);
            return $"RF{digits3to4}{outputString}";
        } catch(Exception ex) {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    private int GetTenthDigit(double? orderAmount) {
        if(orderAmount <= 0) {
            throw new InvalidDataException("Invalid input, order amount");
        }

        int total = (orderAmount.Value * 100).ToString().Reverse().Select((digit, index) => (digit - '0') * (index % 3 == 0 ? 1 : index % 3 == 1 ? 7 : 3)).Sum();
        return total % 8;
    }

    private string GetDigits3to4(string outputString) {
        string digits27 = outputString + "271500";
        BigInteger digits27Int = BigInteger.Parse(digits27);
        int checkDigit = 98 - (int)(digits27Int % 97);
        return checkDigit.ToString("D2");
    }

}