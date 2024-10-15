using Infra.CrossCutting.Email.Interfaces;
using System;

namespace Infra.CrossCutting.Email
{
    public class EmailConfiguration : IEmailConfiguration
    {
        public Guid Id { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpHost { get; set; }
        public bool SmtpEnableCrypto { get; set; }
        public string Login { get; set; }
        public string NameExibition { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }

        public EmailConfiguration()
        {
            Id = Guid.NewGuid();
            SmtpPort = 587;
            SmtpEnableCrypto = true;
            SmtpHost = "smtp.sendgrid.net";
            Login = "apikey";
            Email = "naoresponda@serverconstruaapp.com";
            NameExibition = "Não Responda - Construa App";
            Password = "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";
            Description = "Envio de e-mail do sistema Construa App";
        }
    }
}
