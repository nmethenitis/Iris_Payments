namespace IrisPayments.Helpers.Interfaces;
public interface IEmailSender {
    Task SendEmailAsync(string to, string cc, string subject, string body);
}
