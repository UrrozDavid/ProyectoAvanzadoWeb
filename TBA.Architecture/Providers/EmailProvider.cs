using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TBA.Core.Settings;
using Microsoft.Extensions.Options;


namespace TBA.Architecture.Providers
{
    public class EmailProvider
    {
        private readonly EmailSettings _emailSettings;

        public EmailProvider(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                string from = _emailSettings.From;
                string password = _emailSettings.Password;

                using var message = new MailMessage();
                message.From = new MailAddress(from);
                message.To.Add(new MailAddress(to));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                message.Priority = MailPriority.Normal;

                using var client = new SmtpClient("smtp.office365.com", 587)
                {
                    Credentials = new System.Net.NetworkCredential(from, password),
                    EnableSsl = true
                };

                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}