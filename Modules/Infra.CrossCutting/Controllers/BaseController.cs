using Infra.CrossCutting.Auth;
using Infra.CrossCutting.Notification.Enum;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Infra.CrossCutting.Controllers
{

    public abstract class BaseController : ControllerBase
    {
        private readonly DomainNotificationHandler _messageHandler;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notification"></param>
        protected BaseController(INotificationHandler<DomainNotification> notification)
        {
            _messageHandler = (DomainNotificationHandler)notification;
        }

        protected IActionResult InternalServerError(Exception ex) => StatusCode((int)HttpStatusCode.InternalServerError, ex);

        protected AuthLogged GetUserLogged()
        {
            var userLogger = new AuthLogged()
            {
                Id = -1,
                Email = "",
                Name = "",
                IsAdmin = ""
            };

            if (User != null && User.Claims != null)
            {
                var userId = User.Claims.Where(p => p.Type == AuthRules.UserId).FirstOrDefault();
                var userName = User.Claims.Where(p => p.Type == AuthRules.UserName).FirstOrDefault();
                var userEmail = User.Claims.Where(p => p.Type == AuthRules.UserEmail).FirstOrDefault();
                var userRole = User.Claims.Where(p => p.Type == AuthRules.UserRole).FirstOrDefault();

                if (userId != null)
                    userLogger.Id = int.Parse(userId.Value);

                if (userName != null)
                    userLogger.Name = userName.Value;

                if (userEmail != null)
                    userLogger.Email = userEmail.Value;

                if (userRole != null)
                    userLogger.IsAdmin = userRole.Value;
            }

            return userLogger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool HasNotification()
        {
            return _messageHandler.HasNotifications();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected IActionResult NotificationBusiness()
        {
            var notifications = _messageHandler.GetNotifications();
            var domainNotificationType = notifications?.FirstOrDefault()?.DomainNotificationType;
            domainNotificationType = (domainNotificationType != null) ? domainNotificationType : DomainNotificationType.BadRequest;

            return new JsonResult(new ExceptionResponse(notifications?.ToList(), (HttpStatusCode)domainNotificationType))
            {
                StatusCode = (int) domainNotificationType
            };
        }

        protected IActionResult NotificationBusinessOK<T>(T result = default(T))
        {
            var notifications = _messageHandler.GetNotifications();

            return Ok(new Result<T>
            {
                Error = true,
                Data = result,
                Messages = notifications?.ToList().Select(x => x.Value).ToArray()
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected IActionResult HttpResponse(object response, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            if (_messageHandler.HasNotifications())
            {
                var notifications = _messageHandler.GetNotifications();

                var domainNotificationType = notifications?.FirstOrDefault()?.DomainNotificationType;
                if (domainNotificationType != null)
                {
                    return new JsonResult(new ExceptionResponse(notifications.ToList(), (HttpStatusCode) domainNotificationType))
                    {
                        StatusCode = (int)domainNotificationType
                    };
                }
            }

            return new JsonResult(response)
            {
                StatusCode = (int)statusCode
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected IActionResult OkOrDefault<T>(T result = default(T))
        {

            if (!HasNotification())
            {
                return Ok(new Result<T>
                {
                    Error = false,
                    Data = result
                });               
            }

            return NotificationBusinessOK<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected IActionResult OkOrNotFound<T>(T result = default(T))
        {

            if (!HasNotification())
            {
                if (result != null)
                {
                    if (result is bool)
                        return Ok();

                    return Ok(new Result<T>
                    {
                        Error = false,
                        Data = result
                    });
                }

                return NotFound();

            }

            return NotificationBusiness();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected IActionResult OkOrNoContent<T>(T result = default(T))
        {
            if (!HasNotification() && result != null)
            {
                if (result is IEnumerable<T>)
                {
                    if (((ICollection<T>)result).Count > 0)
                    {
                        return Ok(new Result<T>
                        {
                            Error = false,
                            Data = result
                        });
                    }

                    return NoContent();
                }

                return Ok(new Result<T>
                {
                    Error = false,
                    Data = result
                });
            }

            if (!HasNotification() && result == null)
            {
                return NoContent();
            }

            return NotificationBusiness();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected IActionResult AcceptedContent<T>(T result = default(T))
        {
            if (!HasNotification())
            {
                if (result != null)
                    return Accepted(new Result<T>
                    {
                        Error = false,
                        Data = result
                    });

                return NotFound();
            }

            return NotificationBusiness();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected IActionResult AcceptedOrContent<T>(T result = default(T))
        {
            if (!HasNotification())
            {
                if (result != null)
                    return Accepted(new Result<T>
                    {
                        Error = false,
                        Data = result
                    });

                return Accepted();
            }

            return NotificationBusiness();
        }

        protected IActionResult Error(string Message)
            {
            return Ok(new Result() { Error = true, Messages = new string[] { Message } });
            }

        protected IActionResult Error(string[] Messages)
            {
            return Ok(new Result() { Error = true, Messages = Messages });
            }

        protected IActionResult Error(IEnumerable<string> Messages)
            {
            return Error(Messages.ToArray());
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rota"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected IActionResult CreatedContent<T>(string rota, T result = default(T))
        {
            if (!HasNotification())
            {
                if (result != null)
                    return Created(rota, new Result<T>
                    {
                        Error = false,
                        Data = result
                    });

                return Created(rota, new Result
                {
                    Error = false
                });
            }

            return NotificationBusiness();
        }
    }
    public class Result
    {
        public bool Error { get; set; }
        public string[] Messages { get; set; }

    }
    public class Result<T> : Result
    {
        public T Data { get; set; }
    }

}
