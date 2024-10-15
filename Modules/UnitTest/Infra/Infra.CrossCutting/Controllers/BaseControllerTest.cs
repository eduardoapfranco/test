using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Enum;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Xunit;

namespace UnitTest.Infra.Infra.CrossCutting.Controllers
{
    public class BaseControllerTest
    {
        [Fact(DisplayName = "Has notification should return true")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void HasNotificationShouldTrue()
        {
            // arrange
            var notification = new DomainNotificationHandler();

            // act
            notification.Handle(new DomainNotification("", DomainNotificationType.BadRequest),
                new CancellationToken());

            var baseTest = new BaseControllerImplementsTest(notification);

            // assert
            Assert.True(baseTest.HasNotificationTest());
        }

        [Fact(DisplayName = "Has notification should return false")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void HasNotificationShouldFalse()
        {
            // arrange
            var notification = new DomainNotificationHandler();

            // act
            var baseTest = new BaseControllerImplementsTest(notification);

            // assert
            Assert.False(baseTest.HasNotificationTest());
        }

        [Fact(DisplayName = "Notification business should return return json result bad request")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void NotificationBusinessShouldReturnJsonResultBadRequest()
        {
            // arrange
            var notification = new DomainNotificationHandler();

            notification.Handle(new DomainNotification("", DomainNotificationType.BadRequest),
                new CancellationToken());

            // act
            var baseTest = new BaseControllerImplementsTest(notification);
            var result = baseTest.NotificationBusinessTeste();

            // assert
            Assert.NotNull(result);

            //ExceptionResponse
            var jsonResult = (JsonResult)result;

            Assert.IsType<JsonResult>(result);
            Assert.IsType<ExceptionResponse>(jsonResult.Value);
            Assert.Equal(400, jsonResult.StatusCode);

            var resultException = (ExceptionResponse)jsonResult.Value;
            Assert.NotNull(result);
            Assert.Equal("BadRequest", resultException.Error.Code);
        }

        [Fact(DisplayName = "Notification business should return return json result conflict")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void NotificationBusinessShouldReturnJsonResultConflict()
        {
            // arrange
            var notification = new DomainNotificationHandler();

            // act
            notification.Handle(new DomainNotification("", DomainNotificationType.Conflict),
                new CancellationToken());
            var baseTest = new BaseControllerImplementsTest(notification);
            var result = baseTest.NotificationBusinessTeste();

            // assert
            Assert.NotNull(result);
            var jsonResult = (JsonResult)result;

            Assert.IsType<JsonResult>(result);
            Assert.IsType<ExceptionResponse>(jsonResult.Value);
            Assert.Equal(409, jsonResult.StatusCode);

            var resultException = (ExceptionResponse)jsonResult.Value;
            Assert.NotNull(result);
            Assert.Equal("Conflict", resultException.Error.Code);
        }

        [Fact(DisplayName = "Notification business should return return json result")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void NotificationBusinessShouldReturnJsonResult()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            notification.Handle(new DomainNotification("", DomainNotificationType.BadRequest),
                new CancellationToken());
            
            // act
            var baseTest = new BaseControllerImplementsTest(notification);
            var result = baseTest.NotificationBusinessTeste();

            // assert
            Assert.NotNull(result);
            var jsonResult = (JsonResult)result;      

            Assert.IsType<JsonResult>(result);
            Assert.IsType<ExceptionResponse>(jsonResult.Value);
        }

        [Fact(DisplayName = "Http response should return return json result status code parameter")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void HttpResponseShouldReturnJsonResultStatusCodeParameter()
        {
            // arramge
            var notification = new DomainNotificationHandler();
            var baseTest = new BaseControllerImplementsTest(notification);


            // act
            var result = baseTest.HttpResponseTeste(new object(), HttpStatusCode.Accepted);

            // assert
            Assert.NotNull(result);
            var jsonResult = (JsonResult)result;

            Assert.IsType<JsonResult>(result);
            Assert.Equal((int)HttpStatusCode.Accepted, jsonResult.StatusCode);
        }

        [Fact(DisplayName = "Http response should return return json result status code error if exists notification")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void HttpResponseShouldReturnJsonResultStatusCodeErrorIfExistsNotification()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            notification.Handle(new DomainNotification("", DomainNotificationType.BadRequest),
                new CancellationToken());
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.HttpResponseTeste(new object(), HttpStatusCode.Accepted);

            // assert
            Assert.NotNull(result);
            var jsonResult = (JsonResult)result;

            Assert.IsType<JsonResult>(result);
            Assert.IsType<ExceptionResponse>(jsonResult.Value);
            Assert.Equal(400, jsonResult.StatusCode);

            var resultException = (ExceptionResponse)jsonResult.Value;
            Assert.NotNull(result);
            Assert.Equal("BadRequest", resultException.Error.Code);
        }

        [Fact(DisplayName = "Http response should return return json result status code error if not exists notification")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void HttpResponseShouldReturnJsonResultStatusCodeErrorIfNotExistsNotification()
        {
            // arrange
            var notification = new Mock<DomainNotificationHandler>();
            var baseTest = new BaseControllerImplementsTest(notification.Object);

            // act
            var result = baseTest.HttpResponseTeste(new object(), HttpStatusCode.Accepted);

            // assert
            Assert.NotNull(result);
            var jsonResult = (JsonResult)result;

            Assert.IsType<JsonResult>(result);
            Assert.Equal(202, jsonResult.StatusCode);
        }

        [Fact(DisplayName = "Ok or not found should return result status code OK")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void OkOrNotFoundShouldResultStatusCodeOK()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.OkOrNotFoundTeste(new object());

            // assert
            Assert.IsType<OkObjectResult>(result);

            var objectResult = (OkObjectResult)result;

            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.NotNull(objectResult.Value);
        }

        [Fact(DisplayName = "Ok or not found should return result status code OK if Bool")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void OkOrNotFoundShouldResultStatusCodeOKIfBool()
        {
            
            // arrange
            var notification = new DomainNotificationHandler();
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.OkOrNotFoundTeste(true);

            // assert
            Assert.IsType<OkResult>(result);

            var objectResult = (OkResult)result;

            Assert.IsType<OkResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Ok or not found should return result status code not found")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void OkOrNotFoundShouldResultStatusCodeNotFound()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.OkOrNotFoundTeste(default(object));

            // assert
            Assert.IsType<NotFoundResult>(result);

            var objectResult = (NotFoundResult)result;

            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Ok or not found should return result status code bad request if exists notification")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void OkOrNotFoundShouldResultStatusCodeBadRequestIfExistsNotification()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            notification.Handle(new DomainNotification("", DomainNotificationType.BadRequest),
                new CancellationToken());
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.OkOrNotFoundTeste(default(object));

            // assert
            Assert.IsType<JsonResult>(result);

            var objectResult = (JsonResult)result;

            Assert.IsType<JsonResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Ok or not content should return result status code no content")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void OkOrNotContentShouldResultStatusCodeNoContent()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.OkOrNotContentTest(default(object));

            // assert
            Assert.IsType<NoContentResult>(result);

            var objectResult = (NoContentResult)result;

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Ok or not content should return result status code no content for empty list")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void OkOrNotContentShouldResultStatusCodeNoContentForEmptyList()
        {
            // arrabge
            var notification = new DomainNotificationHandler();
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.OkOrNotContentTest(default(object));

            // assert
            Assert.IsType<NoContentResult>(result);

            var objectResult = (NoContentResult) result;

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Ok or not content should return result status code OK")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void OkOrNotContentShouldResultStatusCodeOK()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.OkOrNotContentTest(new object());

            // assert
            Assert.IsType<OkObjectResult>(result);

            var objectResult = (OkObjectResult)result;

            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Ok or not content should return result status code OK greather than zero")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void OkOrNotContentShouldResultStatusCodeOKForListGreatherThanZero()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.OkOrNotContentTest(new List<object>()
            {
                new object()
            });

            // assert
            Assert.IsType<OkObjectResult>(result);

            var objectResult = (OkObjectResult)result;

            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Ok or not content should return result status code bad request if exist notifification")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void OkOrNotContentShouldResultStatusCodeBadRequestIfExistNotification()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            notification.Handle(new DomainNotification("", DomainNotificationType.BadRequest),
                new CancellationToken());
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.OkOrNotContentTest(default(object));

            // assert
            Assert.IsType<JsonResult>(result);

            var objectResult = (JsonResult)result;

            Assert.IsType<JsonResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Accepted content should return return not found")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void AcceptedContentShouldReturnNotFound()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.AcceptedContentTest(default(object));

            // assert
            Assert.IsType<NotFoundResult>(result);

            var objectResult = (NotFoundResult)result;

            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Accepted content should return return accepted")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void AcceptedContentShouldReturnAccepted()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.AcceptedContentTest(new object());

            // assert
            Assert.IsType<AcceptedResult>(result);

            var objectResult = (AcceptedResult)result;

            Assert.IsType<AcceptedResult>(result);
            Assert.Equal((int)HttpStatusCode.Accepted, objectResult.StatusCode);
            Assert.NotNull(objectResult.Value);
        }

        [Fact(DisplayName = "Accepted content should return return result status code bad request if exist notification")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void AcceptedContentShouldReturnResultStatusCodeBadRequestIfExistNotification()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            notification.Handle(new DomainNotification("", DomainNotificationType.BadRequest),
                new CancellationToken());
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.AcceptedContentTest(default(object));

            // assert
            Assert.IsType<JsonResult>(result);

            var objectResult = (JsonResult)result;

            Assert.IsType<JsonResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Accepted or content should return return accepted without value")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void AcceptedOrContentShouldReturnAcceptedWithoutValue()
        {
            // arramge
            var notification = new DomainNotificationHandler();
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.AcceptedOrContentTest(default(object));

            // assert
            Assert.IsType<AcceptedResult>(result);

            var objectResult = (AcceptedResult)result;

            Assert.IsType<AcceptedResult>(result);
            Assert.Null(objectResult.Value);
            Assert.Equal((int)HttpStatusCode.Accepted, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Accepted content should return return accepted wit value")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void AcceptedContentShouldReturnAcceptedWithValue()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.AcceptedOrContentTest(new object());

            // assert
            Assert.IsType<AcceptedResult>(result);

            var objectResult = (AcceptedResult)result;

            Assert.IsType<AcceptedResult>(result);
            Assert.Equal((int)HttpStatusCode.Accepted, objectResult.StatusCode);
            Assert.NotNull(objectResult.Value);
        }

        [Fact(DisplayName = "Accepted or content should return return result status code bad request if exist notification")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void AcceptedOrContentShouldReturnResultStatusCodeBadRequestIfExistNotification()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            notification.Handle(new DomainNotification("", DomainNotificationType.BadRequest),
                new CancellationToken());
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.AcceptedOrContentTest(default(object));

            // assert
            Assert.IsType<JsonResult>(result);

            var objectResult = (JsonResult)result;

            Assert.IsType<JsonResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Created content should return return result status code bad request if exist notification")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void CreatedContentShouldReturnResultStatusCodeBadRequestIfExistNotification()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            notification.Handle(new DomainNotification("", DomainNotificationType.BadRequest),
                new CancellationToken());
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.CreatedContentTest("Rota", default(object));

            // assert
            Assert.IsType<JsonResult>(result);

            var objectResult = (JsonResult)result;

            Assert.IsType<JsonResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Created content should return return result created success")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void CreatedContentShouldReturnResultCreatedSuccess()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.CreatedContentTest("Rota", default(object));

            // assert
            Assert.IsType<CreatedResult>(result);

            var objectResult = (CreatedResult)result;

            Assert.IsType<CreatedResult>(result);
            Assert.Equal(201, objectResult.StatusCode);

            var resultValue = (Result)objectResult.Value;

            Assert.False(resultValue.Error);
}
        
        [Fact(DisplayName = "Created content should return return result created success with data")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void CreatedContentShouldReturnResultCreatedSuccessWithData()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            var baseTest = new BaseControllerImplementsTest(notification);
            var objetoResultadoParaTeste = new Result();

            // act
            var result = baseTest.CreatedContentTest("Rota", objetoResultadoParaTeste);

            // assert
            Assert.IsType<CreatedResult>(result);

            var objectResult = (CreatedResult)result;

            Assert.IsType<CreatedResult>(result);
            Assert.Equal(201, objectResult.StatusCode);

            var resultValue = (Result<Result>)objectResult.Value;

            Assert.False(resultValue.Error);
            Assert.Equal(objetoResultadoParaTeste.Error, resultValue.Data.Error);
        }

        [Fact(DisplayName = "OK or default should return return result if exist notification")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void OKOrDefaultShouldReturnResultIfExistNotification()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            notification.Handle(new DomainNotification("Error no cadastro", DomainNotificationType.BadRequest),
                new CancellationToken());
            notification.Handle(new DomainNotification("Error no cadastro", DomainNotificationType.BadRequest),
                new CancellationToken());
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.OkOrDefaultTest(default(object));
            var jsonResult = (OkObjectResult)result;
            var dataResult = (Result<object>)jsonResult.Value;

            // assert
            Assert.IsType<OkObjectResult>(result); 
            Assert.Equal(200, jsonResult.StatusCode);
            Assert.True(dataResult.Error);
            Assert.Null(dataResult.Data);
            Assert.Equal(2, dataResult.Messages.Count());
            Assert.Equal("Error no cadastro", dataResult.Messages[0]);
        }

        [Fact(DisplayName = "OK or default should return return result if not exist notification")]
        [Trait("[Infra.CrossCutting]-BaseController", "Controllers")]
        public void OKOrDefaultShouldReturnResultIfNotExistNotification()
        {
            // arrange
            var notification = new DomainNotificationHandler();        
            var baseTest = new BaseControllerImplementsTest(notification);

            // act
            var result = baseTest.OkOrDefaultTest(new string[] { "teste" });
            var jsonResult = (OkObjectResult)result;
            var dataResult = (Result<string[]>)jsonResult.Value;

            // assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, jsonResult.StatusCode);
            Assert.False(dataResult.Error);
            Assert.NotNull(dataResult.Data);
        }
    }

    public class BaseControllerImplementsTest : BaseController
    {
        public BaseControllerImplementsTest(INotificationHandler<DomainNotification> notification) : base(notification)
        {
        }

        public bool HasNotificationTest()
        {
            return HasNotification();
        }

        public IActionResult NotificationBusinessTeste()
        {
            return NotificationBusiness();
        }

        public IActionResult HttpResponseTeste(object response, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return HttpResponse(response, statusCode);
        }

        public IActionResult OkOrNotFoundTeste<T>(T result = default(T))
        {
            return OkOrNotFound(result);
        }

        public IActionResult OkOrNotContentTest<T>(T result = default(T))
        {
            return OkOrNoContent(result);
        }

        public IActionResult AcceptedContentTest<T>(T result = default(T))
        {
            return AcceptedContent(result);
        }

        public IActionResult OkOrDefaultTest<T>(T result = default(T))
        {
            return OkOrDefault<T>(result);
        }

        public IActionResult AcceptedOrContentTest<T>(T result = default(T))
        {
            return AcceptedOrContent(result);
        }
        public IActionResult CreatedContentTest<T>(string rota, T result = default(T))
        {
            return CreatedContent(rota, result);
        }

    }
}
