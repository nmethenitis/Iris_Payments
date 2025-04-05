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
        string orgNumber = _config.GetValue<string>("AppSettings:Dias:OrgNumber"); //Kwdikos Dikaiouxou Organismou : digits 5-9
        try {
            string RFCode = string.Empty;
            StringBuilder sb = new StringBuilder(orgNumber);
            int tenthDigit = GetTenthDigit(amount); //digit 10
            sb.Append(tenthDigit);
            string customerID = string.Format("{0:D8}", customerId);
            int zeroPaddingNumber = 15 - customerID.Length;
            if (zeroPaddingNumber > 0) {
                int unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                string unixTimestampString = unixTimestamp.ToString();
                string unixTimestampLast8Digits = unixTimestampString.Substring(unixTimestampString.Length - zeroPaddingNumber);
                sb.Append(unixTimestampLast8Digits);
            }
            sb.Append(customerID);
            string outputstring21Digits = sb.ToString(); //Afto tha epistrepsoume + ta 4 prota digits
            string digits3to4 = GetDigits3to4(outputstring21Digits);
            RFCode = string.Format("RF{0}{1}", digits3to4, outputstring21Digits);
            return RFCode;
        } catch (Exception ex) {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    private string GetZeroString(int zeroesNumber) {
        StringBuilder zeroes = new StringBuilder("");
        for (int i = 0;i < zeroesNumber;i++) {
            zeroes.Append("0");
        }
        return zeroes.ToString();
    }

    private int GetTenthDigit(double? orderAmount) {
        if (orderAmount > 0) {
            double? orderAmountCents = orderAmount * 100;
            string orderAmountCentsString = orderAmountCents.ToString();
            int counter = 0;
            int total = 0;
            for (int i = orderAmountCentsString.Length;i > 0;i--) {
                string currentDigit = orderAmountCentsString.Substring(i - 1, 1);
                int currentDigitInt = -1;
                int.TryParse(currentDigit, out currentDigitInt);
                int multiplyBy = -1;
                if (counter % 3 == 0)
                    multiplyBy = 1;
                else if (counter % 3 == 1)
                    multiplyBy = 7;
                else if (counter % 3 == 2)
                    multiplyBy = 3;
                if (currentDigitInt >= 0 && multiplyBy >= 0) {
                    total += currentDigitInt * multiplyBy;
                } else {
                    throw new InvalidDataException("Invalid input");
                }
                counter++;
            }
            return total % 8;
        } else {
            throw new InvalidDataException("Invalid input, order amount");
        }
    }

    private string GetDigits3to4(string outputstring21Digits) {
        string digits27 = outputstring21Digits + "271500";
        BigInteger digits27Int = new BigInteger();
        BigInteger.TryParse(digits27, out digits27Int);
        BigInteger ypoloipo = digits27Int % 97;
        int psifioElegxou = 98 - (int)ypoloipo;
        if (psifioElegxou >= 10)
            return psifioElegxou.ToString();
        else
            return "0" + psifioElegxou;
    }
}