using IrisPayments.Helpers.Interfaces;
using System.Net;
using System.Net.Mail;

namespace IrisPayments.Helpers.Services;
public class EmailSenderService : IEmailSender {

    private readonly IConfiguration _config;
    private readonly string ERROR_EMAIL_SUBJECT = "DCT REST Payment API Exception";

    public EmailSenderService(IConfiguration config) {
        _config = config;
    }
    public async Task SendEmailAsync(string to, string cc, string subject, string body) {
        try {
            using (var smtp = new SmtpClient()) {
                var credential = new NetworkCredential {
                    UserName = _config.GetValue<string>("AppSettings:SMTPSettings:Username"),
                    Password = _config.GetValue<string>("AppSettings:SMTPSettings:Password")
                };
                smtp.Credentials = credential;
                smtp.Host = _config.GetValue<string>("AppSettings:SMTPSettings:Host");
                var message = new MailMessage {
                    From = new MailAddress(_config.GetValue<string>("AppSettings:SMTPSettings:Sender")),
                    Body = body,
                    Subject = subject,
                    IsBodyHtml = true
                };
                string recipientEmail = !String.IsNullOrEmpty(to) ? to : _config.GetValue<string>("AppSettings:SMTPSettings:DefaultRecipient");
                List<string> recipients = recipientEmail.Split(';').ToList();
                foreach (string recipient in recipients) {
                    message.To.Add(new MailAddress(recipient));
                }
                if(cc != null) {
                    List<string> recipientsCC = cc.Split(';').ToList();
                    foreach(string recipient in recipientsCC) {
                        message.CC.Add(new MailAddress(recipient));
                    }
                }
                await smtp.SendMailAsync(message);
            }
        } catch (Exception ex) { }
    }
}
