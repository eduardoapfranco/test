using Infra.CrossCutting.Email.Interfaces;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infra.CrossCutting.Email.Services
{
    public class EmailSendService : IEmailSendService
    {
        public async Task SendEmailAsync(EmailSendInput input, EmailConfiguration emailConfiguration)
        {
            try
            {
                var smtpClient = new SmtpClient(emailConfiguration.SmtpHost, emailConfiguration.SmtpPort);
                var mailAddressFrom = new MailAddress(emailConfiguration.Email, emailConfiguration.NameExibition, Encoding.UTF8);


                var mailMessage = new MailMessage { From = mailAddressFrom };

                if (input.Cco != null && input.Cco.Any())
                {
                    foreach (var receiver in input.Cco)
                    {
                        mailMessage.Bcc.Add(new MailAddress(receiver.Email.Trim(), input.Name, Encoding.UTF8));
                    }

                }
                else if (!String.IsNullOrWhiteSpace(input.Email))
                {
                    mailMessage.To.Add(new MailAddress(input.Email.Trim(), input.Name, Encoding.UTF8));
                }


                mailMessage.Subject = input.Subject;
                mailMessage.SubjectEncoding = Encoding.UTF8;

                mailMessage.Body = input.Body;
                mailMessage.BodyEncoding = Encoding.UTF8;
                mailMessage.IsBodyHtml = input.BodyHtml;

                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new NetworkCredential(emailConfiguration.Login, emailConfiguration.Password);
                smtpClient.EnableSsl = emailConfiguration.SmtpEnableCrypto;
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException e)
            {
                throw new Exception($"Falha de envio de e-mail. {e.Message}");
            }
        }
    }
}
