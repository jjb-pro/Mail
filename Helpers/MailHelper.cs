using Mail.Model;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Mail.Helpers;

public static partial class MailHelper
{
    public static async Task SendMail(Account account, string[] recipients, string subject, Priority priority, string messageBody, CancellationToken cancellationToken)
    {
        SmtpClient smtpClient = account.Port != 0 ? new(account.Smtp, (int)account.Port) : new(account.Smtp);
        smtpClient.Credentials = new System.Net.NetworkCredential(account.EmailAddress, account.Password);
        smtpClient.EnableSsl = account.EnableSSL;

        MailAddress from = new(account.EmailAddress);

        foreach (string recipient in recipients)
        {
            // cancel if cancellation requested
            cancellationToken.ThrowIfCancellationRequested();

            MailAddress to = new(recipient);
            MailMessage mail = new(from, to)
            {
                // set subject and encoding
                Subject = subject,
                SubjectEncoding = Encoding.UTF8,

                Priority = (MailPriority)priority,

                // set body-message and encoding
                Body = messageBody,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true
            };

            await smtpClient.SendMailAsync(mail, cancellationToken);
        }
    }

    [GeneratedRegex("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])")]
    private static partial Regex EmailRegex();

    public static bool IsValidEmailAddress(string emailAddress) => EmailRegex().IsMatch(emailAddress);
}

