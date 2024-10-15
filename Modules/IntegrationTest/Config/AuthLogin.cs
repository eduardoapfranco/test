using Application.AppServices.UserApplication.Input;
using Application.AppServices.UserApplication.ViewModel;
using System.Threading.Tasks;

namespace IntegrationTest.Config
{
    public static class AuthLogin
    {
        public static string Email => "construaapp@gmail.com";
        public static string Password => "123456";


        public static async Task<string> GetTokenUser(TestContext testContext)
        {
            var request = new
            {
                Url = "/api/v1/login"
            };

            var input = new UserLoginInput()
            {
                Email = Email,
                Password = Password
            };

            var response = await testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserViewModel>.GetResponse(response);
            return result.Data.Token;
        }

        public static async Task<UserViewModel> GetUser(TestContext testContext)
        {
            var request = new
            {
                Url = "/api/v1/login"
            };

            var input = new UserLoginInput()
            {
                Email = Email,
                Password = Password
            };

            var response = await testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserViewModel>.GetResponse(response);
            return result.Data;
        }
    }
}
