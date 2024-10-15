using UnitTest.Application.UserApplication.Faker;
using Xunit;

namespace UnitTest.Application.UserApplication
    {
    public class UserInputValidatorTest
    {

        public UserInputValidatorTest()
        {
            
        }

        [Fact(DisplayName = "Shoud return invalid password length insert mobile async")]
        [Trait("[Application.AppServices]-UserApplication", "Application-InsertMobileAsync")]
        public void ShouldReturnInvalidPasswordLengthInsertMobileAsync()
            {
            // arrange
            var userInput = UserFaker.CreateUserInputWithPasswordInvalid();
           
            // act
            var result = userInput.IsValid();

            // assert
            Assert.False(result);
            Assert.Equal("LengthValidator",
                userInput.ValidationResult.Errors[0].ErrorCode);
            }

        [Fact(DisplayName = "Shoud return valid password length insert mobile async")]
        [Trait("[Application.AppServices]-UserApplication", "Application-InsertMobileAsync")]
        public void ShouldReturnValidPasswordLengthInsertMobileAsync()
            {
            // arrange
            var userInput = UserFaker.CreateUserInputWithPasswordInvalid();
            userInput.Password = "123456";
            userInput.PasswordConfirm = "123456";

            // act
            var result = userInput.IsValid();

            // assert
            Assert.True(result);
            Assert.Empty(userInput.ValidationResult.Errors);
            }
    }
}
