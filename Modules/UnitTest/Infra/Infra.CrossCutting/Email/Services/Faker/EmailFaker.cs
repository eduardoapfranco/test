using Infra.CrossCutting.Email;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest.Infra.Infra.CrossCutting.Email.Services.Faker
{
    internal static class EmailFaker
    {
        public static EmailSendInput CreateSendInput()
        {
            return new EmailSendInput()
            {
                Body = "<h1>Hello! This is a e-mail unit test.</h1>",
                BodyHtml = true,
                Cco = new List<CcoInput>()
                    {
                        new CcoInput("construaapp@gmail.com", "Name 1") {},
                        new CcoInput("construaapp@gmail.com", "Name 2") {}
                },
                Email = "construaapp@gmail.com",
                Name = "Jhony Test",
                Subject = "Unit Test - Send E-mail"
            };
        }
    }
}
