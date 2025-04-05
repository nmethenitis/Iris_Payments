namespace IrisPayments.Helpers;
public class Constants {

    public class MessageType {
        public static readonly string DpgOnRequest = "DPG_ONRequest";
        public static readonly string DpgOnResponse = "DPG_ONResponse";
        public static readonly string DpgOn = "DPG_ON";
    }

    public class PaymentStatus {
        public static readonly string Accepted = "ACCEPTED";
        public static readonly string Rejected = "REJECTED";
    }

    public class PaymentRejectionCode {
        public static readonly string Duplication = "AM05";
        public static readonly string NotSpecifiedReasonAgentGenerated = "MS03";
        public static readonly string TooLowAmount = "AM06";
        public static readonly string InconsistentWithEndCustomer = "BE01";
        public static readonly string UnknownEndCustomer = "BE06";
        public static readonly string InvalidDate = "DT01";
    }

    public class PaymentRejectionReason {
        public static readonly string Duplication = "Duplication";
        public static readonly string Amount = "Amount missmatch";
        public static readonly string RFNotFound = "RF Not Found";
        public static readonly string OrgNotFound = "Organization Not Found";
    }
}