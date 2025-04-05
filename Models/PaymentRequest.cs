namespace IrisPayments.Models;
public class PaymentRequest {
    public string Env { get; set; }
    public string MsgType { get; set; }
    public Incomingpayments[] IncomingPayments { get; set; }
}
public class Incomingpayments {
    public string RID { get; set; }
    public string LobID { get; set; }
    public string ActorID { get; set; }
    public string TransactionID { get; set; }
    public string RI_MID { get; set; }
    public double PaidAmount { get; set; }
    public string DebtorName { get; set; }
    public string DebtorBankBIC { get; set; }
    public string PaymentDtTm { get; set; }
    public string BkDate { get; set; }
    public string RemittanceInformation { get; set; }
    public string CreationDtTm { get; set; }
}