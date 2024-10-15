using Application.AppServices.ReminderApplication.ViewModel;
using IntegrationTest.Config;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTest.Scenarios.Reminder
{
    public class ReminderControllerIntegrationTests
        {
        private readonly TestContext _testContext;

        public ReminderControllerIntegrationTests()
            {
            _testContext = new TestContext();
            }

        [Fact(DisplayName = "Should return ok when requesting all reminders for currently logged-in user")]
        [Trait("[IntegrationTest]-ReminderController", "AllReminders")]
        public async Task AllReminders()
            {
            // arrange
            var request = new
                {
                Url = String.Format("/api/v1/reminder/all")
                };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<ReminderViewModel>>.GetResponse(response);
            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);

            }
        }
    }
