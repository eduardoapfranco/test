using System;

namespace Infra.CrossCutting.Email.Interfaces
{
    public interface IEmailConfiguration
    {
        Guid Id { get; set; }
        int SmtpPort { get; set; }
        string SmtpHost { get; set; }
        bool SmtpEnableCrypto{ get; set; }
        string Login { get; set; }
        string NameExibition { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string Description { get; set; }
    }
}
