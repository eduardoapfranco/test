using System.Threading.Tasks;

namespace Infra.CrossCutting.Email.Interfaces
{
    public interface IEmailSendService
    {
        Task SendEmailAsync(EmailSendInput input, EmailConfiguration email);
    }
}
