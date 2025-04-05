using IrisPayments.Helpers;
using System.Text.Json.Serialization;

namespace IrisPayments.Models;
public class PaymentResponse {
    public PaymentResponse() {
        incomingPaymentsStatus = new IncomingPaymentsStatus[1];
        msgType = Constants.MessageType.DpgOnResponse;
    }
    public string env { get; set; }
    public string msgType { get; set; }
    public IncomingPaymentsStatus[] incomingPaymentsStatus { get; set; }
}

public class IncomingPaymentsStatus {
    public string status { get; set; }
    public string rejectionCode { get; set; }
    [JsonPropertyName("RID")]
    public string RID { get; set; }
}