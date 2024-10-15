using Infra.CrossCutting.Controllers;
using System.Collections.Generic;
using Xunit;

namespace UnitTest.Infra.Infra.CrossCutting.Controllers
{
    public class ErrorsTest
    {
        [Fact(DisplayName = "Should return to validate propperties errors")]
        [Trait("[Infra.CrossCutting]-Errors", "Controllers")]
        public void ValidatePropertiesErrors()
        {
            // arrange
            var message = new List<string>()
            {
                "Errors"
            };

            // act
            var errors = new Errors()
            {
                Code = "CodeTeste",
                Message = message,
                Target = "TargetTeste"
            };

            // assert
            Assert.Equal("CodeTeste", errors.Code);
            Assert.Equal("TargetTeste", errors.Target);
            var castMessage = (List<string>)errors.Message;
            Assert.Equal(message, castMessage);
        }
    }
}
