using Infra.CrossCutting.Email.Interfaces;
using System.Collections.Generic;

namespace Infra.CrossCutting.Email
{
    public class EmailSendInput : IEmailSendInput
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool BodyHtml { get; set; }
        public IEnumerable<CcoInput> Cco { get; set; }
    }
}
