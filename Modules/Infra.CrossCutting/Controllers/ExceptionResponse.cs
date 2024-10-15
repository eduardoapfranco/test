using Infra.CrossCutting.Notification.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace Infra.CrossCutting.Controllers
{

    /// <summary>
    /// 
    /// </summary>
    public class ExceptionResponse
    {
        private ExceptionResponse()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public ExceptionResponse(Exception exception, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            var response = GetCompleteException(exception, statusCode);
            Error = response.Error;
            InnerError = response.InnerError;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public ExceptionResponse(List<DomainNotification> exception, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            var response = GetCompleteException(exception, statusCode);
            Error = response.Error;
            InnerError = response.InnerError;
        }

        /// <summary>
        ///     Response error
        /// </summary>
        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public Errors Error { get; private set; }

        /// <summary>
        ///     Response inner error
        /// </summary>
        [JsonProperty("innererror", NullValueHandling = NullValueHandling.Ignore)]
        public ExceptionResponse InnerError { get; private set; }

        private ExceptionResponse GetCompleteException(Exception exception, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            var response = new ExceptionResponse
            {
                Error = new Errors
                {
                    Message = exception.Message,
                    Code = statusCode.ToString()
                },
                InnerError = exception.InnerException != null
                    ? GetCompleteException(exception.InnerException)
                    : default(ExceptionResponse)
            };

            return response;
        }

        private ExceptionResponse GetCompleteException(List<DomainNotification> validationErrors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            var response = new ExceptionResponse
            {
                Error = new Errors
                {
                    Message = validationErrors,
                    Code = statusCode.ToString()
                }
            };

            return response;
        }
    }
}

