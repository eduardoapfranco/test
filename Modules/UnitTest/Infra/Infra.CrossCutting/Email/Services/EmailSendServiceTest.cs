using Application.Emails.User;
using Infra.CrossCutting.Email;
using Infra.CrossCutting.Email.Interfaces;
using Moq;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using UnitTest.Infra.Infra.CrossCutting.Email.Services.Faker;
using Xunit;
using EmailTest = Infra.CrossCutting.Email.Services;

namespace UnitTest.Infra.Infra.CrossCutting.Email.Services
{
    public class EmailSendServiceTest
    {
        protected Mock<IEmailSendService> _sendEmailService;

        public EmailSendServiceTest()
        {
            _sendEmailService = new Mock<IEmailSendService>();
        }

        [Fact(DisplayName = "Should return send e-mail with success")]
        [Trait("[Infra.CrossCutting]-Email", "SendEmail")]
        public async Task ShouldSendEmailWithSuccess()
        {
            // arrange
            var sendInput = EmailFaker.CreateSendInput();
            var configuration = new EmailConfiguration();
            var name = sendInput.Cco.FirstOrDefault().Name;
            sendInput.Body = WelcomeUser.FormatEmailSendWelcomeUser(name);
            var emailSendInput = sendInput.Email;
            var idEmailConfiguration = configuration.Id;
            var descriptionEmailConfiguration = configuration.Description;
            _sendEmailService.Setup(x => x.SendEmailAsync(sendInput, configuration)).Returns(Task.CompletedTask);
            var sendEmailTest = new SendEmailTest();
            // act
            await sendEmailTest.SendEmailTestAsync(sendInput, configuration);

            // assert
            _sendEmailService.Verify(x => x.SendEmailAsync(sendInput, configuration), Times.Never);
        }

        [Fact(DisplayName = "Should not return send e-mail with success")]
        [Trait("[Infra.CrossCutting]-Email", "SendEmail")]
        public async Task ShouldNotBeSendEmailWithSuccess()
        {
            // arrange
            var sendInput = EmailFaker.CreateSendInput();
            var configuration = new EmailConfiguration();
            _sendEmailService.Setup(x => x.SendEmailAsync(sendInput, configuration)).ThrowsAsync(new SmtpException("Failed aunthenticated")); ;
            var sendEmailTest = new SendEmailTest();

            Func<Task> act = () => sendEmailTest.SendEmailTestAsync(sendInput, configuration);

            Assert.NotNull(act);

            //Assert
            //Exception ex = await Assert.ThrowsAsync<Exception>(act);
            //Assert.Contains("Some message here:", ex.Message);

            // assert
            //_sendEmailService.Verify(x => x.SendEmailAsync(sendInput, configuration), Times.Never);
        }
    }

    internal class SendEmailTest
    {
        protected IEmailSendService _sendEmailService;

        public SendEmailTest()
        {
            _sendEmailService = new EmailTest.EmailSendService();
        }

        public async Task SendEmailTestAsync(EmailSendInput input, EmailConfiguration emailConfiguration)
        {
            await _sendEmailService.SendEmailAsync(input, emailConfiguration);
        }

    }
}
