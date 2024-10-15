using System;
using System.Collections.Generic;
using System.Text;

namespace Infra.CrossCutting.Email.Interfaces
{
    public interface IEmailSendInput
    {
        string Email { get; set; }
        string Name { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
        bool BodyHtml { get; set; }
        IEnumerable<CcoInput> Cco { get; set; }
    }
}
