using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Enum;
using Infra.CrossCutting.Notification.Model;
using System;
using System.Collections.Generic;
using Xunit;

namespace UnitTest.Infra.Infra.CrossCutting.Controllers
{
    public class ExceptionResponseTest
    {
        [Fact(DisplayName = "Construct should return create with base in exception")]
        [Trait("[Infra.CrossCutting]-ExceptionResponse", "Controllers")]
        public void ConstructShouldCreateWithBaseInException()
        {
            // arrange && act
            var exceptionResponse = new ExceptionResponse(new Exception("ExPrincipal", new Exception("InnerEx")));

            // assert
            Assert.Equal("ExPrincipal", (string)exceptionResponse.Error.Message);
            Assert.Equal("BadRequest", exceptionResponse.Error.Code);
            Assert.Equal("InnerEx", exceptionResponse.InnerError.Error.Message);
            Assert.Equal("BadRequest", exceptionResponse.InnerError.Error.Code);
        }

        [Fact(DisplayName = "Construct should return create with base in exception without inner exception")]
        [Trait("[Infra.CrossCutting]-ExceptionResponse", "Controllers")]
        public void ConstructShouldCreateWithBaseInExceptionWithoutInnerException()
        {
            // arrange && act
            var exceptionResponse = new ExceptionResponse(new Exception("ExPrincipal"));

            // assert
            Assert.Equal("ExPrincipal", (string)exceptionResponse.Error.Message);
            Assert.Equal("BadRequest", exceptionResponse.Error.Code);
            Assert.Null(exceptionResponse.InnerError);
        }

        [Fact(DisplayName = "Construct should return create with base in domain notification")]
        [Trait("[Infra.CrossCutting]-ExceptionResponse", "Controllers")]
        public void ConstructShouldCreateWithBaseInDomainNotification()
        {
            // arrange
            var listDomainNotifications = new List<DomainNotification>()
            {
                new DomainNotification("", DomainNotificationType.BadRequest)
            };

            // act
            var exceptionResponse = new ExceptionResponse(listDomainNotifications);

            // assert
            Assert.Equal(listDomainNotifications, (List<DomainNotification>)exceptionResponse.Error.Message);
            Assert.Equal("BadRequest", exceptionResponse.Error.Code);
            Assert.Null(exceptionResponse.InnerError);
        }
    }
}
