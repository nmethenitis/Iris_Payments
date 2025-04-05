using IrisPayments.Data.Interfaces;
using IrisPayments.Data.Models;
using IrisPayments.Helpers.Interfaces;
using IrisPayments.Models;

namespace IrisPayments.Helpers.Services;
public class PaymentLogsHelperService : IPaymentLogsHelper {

    private readonly IPaymentLogs _paymentLogsService;
    private readonly ILogger<PaymentLogsHelperService> _logger;
    public PaymentLogsHelperService(IPaymentLogs paymentLogs, ILogger<PaymentLogsHelperService> logger) {
        _paymentLogsService = paymentLogs;
        _logger = logger;
    }

    public async Task<HistoryPaymentCodeResponse> GetLogHistory(string code) {
        try {
            var paymentlogs = await _paymentLogsService.GetAll(code);
            if (paymentlogs.Count > 0) {
                return new HistoryPaymentCodeResponse() {
                    Success = true,
                    PaymentLogs = paymentlogs
                };
            } else {
                return null;
            }
        } catch (Exception ex) {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<PaymentLogs> CreateLog(PaymentCode paymentCode, Incomingpayments incomingPayment) {
        PaymentLogs log = new PaymentLogs();
        if (paymentCode != null) {
            log.OrderId = paymentCode.OrderID;
            log.CustomerId = paymentCode.CustomerID;
        }
        log.ReferenceId = incomingPayment.RID;
        log.LobId = incomingPayment.LobID;
        log.OrganizationId = incomingPayment.ActorID;
        log.TransactionId = incomingPayment.TransactionID;
        log.PaymentCode = incomingPayment.RI_MID;
        log.PaidAmount = incomingPayment.PaidAmount;
        log.DebtorName = incomingPayment.DebtorName;
        log.DebtorBankBIC = incomingPayment.DebtorBankBIC;
        log.PaymentDtTm = incomingPayment.PaymentDtTm;
        log.BkDate = incomingPayment.BkDate;
        log.RemittanceInformation = incomingPayment.RemittanceInformation;
        log.CreationDtTm = incomingPayment.CreationDtTm;
        log.IsPaid = false;
        return log;
    }

    public async Task<PaymentLogs> UpdateLog(PaymentLogs log, bool isPaid) {
        log.IsPaid = isPaid;
        return log;
    }

    public async Task<PaymentLogs> UpdateLog(PaymentLogs log, PaymentResponse paymentResponse, string rejectionReason) {
        log.Status = paymentResponse.incomingPaymentsStatus[0].status;
        log.RejectionCode = paymentResponse.incomingPaymentsStatus[0].rejectionCode;
        log.RejectionReason = rejectionReason;
        return log;
    }

    public async Task<PaymentLogs> UpdateLog(PaymentLogs log, IncomingPaymentsStatus incomingPaymentsStatus, string rejectionReason) {
        log.Status = incomingPaymentsStatus.status;
        log.RejectionCode = incomingPaymentsStatus.rejectionCode;
        log.RejectionReason = rejectionReason;
        return log;
    }
}