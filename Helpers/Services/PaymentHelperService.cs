using IrisPayments.Data.Interfaces;
using IrisPayments.Data.Models;
using IrisPayments.Helpers.Interfaces;
using IrisPayments.Models;
using System.Globalization;
using System.Text;

namespace IrisPayments.Helpers.Services;
public class PaymentHelperService : IPaymentHelper {

    private readonly IPaymentCode _paymentCodeService;
    private readonly IPaymentLogsHelper _paymentLogsHelperService;
    private readonly IPaymentLogs _paymentLogsService;
    private readonly IEmailSender _emailSenderService;
    private readonly ILogger<PaymentHelperService> _logger;
    private readonly IConfiguration _config;

    public PaymentHelperService(IPaymentCode paymentCodeService, IPaymentLogsHelper paymentLogsHelperService, IPaymentLogs paymentLogsService, IEmailSender emailSenderService, ILogger<PaymentHelperService> logger, IConfiguration config) {
        _paymentCodeService = paymentCodeService;
        _paymentLogsHelperService = paymentLogsHelperService;
        _paymentLogsService = paymentLogsService;
        _emailSenderService = emailSenderService;
        _logger = logger;
        _config = config;
    }

    public bool Check(string rf, string amount) {
        throw new System.NotImplementedException();
    }

    public bool Check(PaymentCode paymentIdentity, string amount) {
        if(paymentIdentity != null) {
            amount = amount.Replace(",", ".");
            double diasPrice = 0;
            double.TryParse(amount, NumberStyles.Any, CultureInfo.InvariantCulture, out diasPrice);
            return paymentIdentity.OrderAmount == diasPrice;
        }
        return false;
    }

    public bool Check(PaymentCode paymentIdentity, double amount) => paymentIdentity.OrderAmount == amount;

    public PaymentResponse InitializeResponse(PaymentRequest paymentRequest) {
        PaymentResponse paymentResponse = new PaymentResponse();
        paymentResponse.env = paymentRequest.Env;
        return paymentResponse;
    }

    public IncomingPaymentsStatus CreatePaymentResponse(string RID, bool accept, string rejectionCode = null) {
        IncomingPaymentsStatus incomingPaymentStatus = new IncomingPaymentsStatus();
        incomingPaymentStatus.RID = RID;
        incomingPaymentStatus.status = accept ? Constants.PaymentStatus.Accepted : Constants.PaymentStatus.Rejected;
        incomingPaymentStatus.rejectionCode = accept ? null : rejectionCode;
        return incomingPaymentStatus;
    }

    public async Task SendNotification(PaymentLogs paymentLogs, PaymentCode paymentCode) {
        try {
            String? to = _config.GetValue<string>("AppSettings:SMTPSettings:DefaultRecipient");
            String? cc = _config.GetValue<string>("AppSettings:SMTPSettings:CCRecipient");
            String subject = String.Format($"IRIS payment {paymentLogs.Status}");
            StringBuilder bodySB = new StringBuilder();
            bodySB.Append("<table>");
            if(!paymentLogs.IsPaid) {
                bodySB.Append(String.Format("<tr><td>Reason</td><td><b>{0}</b></td></tr>", paymentLogs.RejectionReason));
            }
            bodySB.Append(String.Format("<tr><td>Κωδικός Οργανισμού</td><td>{0}</td></tr>", paymentLogs.OrganizationId));
            bodySB.Append(String.Format("<tr><td>Όνομα/Επωνυμία DIAS</td><td>{0}</td></tr>", paymentLogs.LobId));
            bodySB.Append(String.Format("<tr><td>Ταυτότητα πληρωμής</td><td>{0}</td></tr>", paymentLogs.PaymentCode));
            bodySB.Append(String.Format("<tr><td>Transaction ID DIAS</td><td>{0}</td></tr>", paymentLogs.TransactionId));
            bodySB.Append(String.Format("<tr><td>Τράπεζα Πελάτη</td><td>{0}</td></tr>", paymentLogs.DebtorBankBIC));
            bodySB.Append(String.Format("<tr><td>Ποσό DIAS</td><td>{0}</td></tr>", paymentLogs.PaidAmount));
            if(paymentCode != null) {
                if(paymentCode.OrderAmount != null) {
                    bodySB.Append(String.Format("<tr><td>Ποσό δημιουργίας RF</td><td>{0}</td></tr>", paymentCode.OrderAmount));
                }
            }
            bodySB.Append("</table>");
            await _emailSenderService.SendEmailAsync(to, cc, subject, bodySB.ToString());
        } catch(Exception ex) {
            _logger.LogError(ex, ex.Message);
        }
    }

    public async Task<IncomingPaymentsStatus> GetPaymentResponse(Incomingpayments incomingPayment) {
        var incomingPaymentsStatus = new IncomingPaymentsStatus();
        var RF = incomingPayment.RI_MID;
        var RID = incomingPayment.RID;
        var amount = incomingPayment.PaidAmount;
        var orgNumber = incomingPayment.ActorID;
        var rejectionReason = String.Empty;
        var paymentCode = await _paymentCodeService.Get(RF);
        var paymentLogs = await _paymentLogsHelperService.CreateLog(paymentCode, incomingPayment);
        var rejectionCode = String.Empty;
        if(paymentCode != null) {
            if(await _paymentLogsService.Exists(paymentCode.Code, RID) || await _paymentLogsService.ExistsPaid(paymentCode.Code)) {
                //TODO Response decline duplication
                rejectionCode = Constants.PaymentRejectionCode.Duplication;
                rejectionReason = Constants.PaymentRejectionReason.Duplication;
                _logger.LogWarning(String.Format("RF Identity {0} rejected due to duplication.", RF));
            } else if(!Check(paymentCode, amount)) {
                //TODO Response decline amount mismatch
                rejectionCode = Constants.PaymentRejectionCode.TooLowAmount;
                rejectionReason = Constants.PaymentRejectionReason.Amount;
                _logger.LogWarning(String.Format("RF Identity {0} rejected due to amount mismatch.", RF));
            }
        } else {
            //TODO Response decline payment id not found
            rejectionCode = Constants.PaymentRejectionCode.NotSpecifiedReasonAgentGenerated;
            rejectionReason = Constants.PaymentRejectionReason.RFNotFound;
            _logger.LogWarning(String.Format("RF Identity {0} rejected due to not found.", RF));
        }
        incomingPaymentsStatus = CreatePaymentResponse(RID, String.IsNullOrEmpty(rejectionCode), rejectionCode);
        paymentLogs = await _paymentLogsHelperService.UpdateLog(paymentLogs, incomingPaymentsStatus, rejectionReason, String.IsNullOrEmpty(rejectionCode));
        await SendNotification(paymentLogs, paymentCode);
        _paymentLogsService.Add(paymentLogs);
        return incomingPaymentsStatus;
    }
}
