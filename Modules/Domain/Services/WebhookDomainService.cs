using Domain.Interfaces.Services;
using Domain.Interfaces.UoW;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Domain.Entities;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.Domain.Services;
using Domain.Interfaces.Repositories;
using System.Threading.Tasks;
using System.Net.Http;
using Domain.Input;
using System;
using System.Web;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Serilog.Context;
using System.Xml;
using System.Collections.Generic;

namespace Domain.Services
    {
    public class WebhookDomainService : DomainService<Webhook, int, IUnitOfWork>, IWebhookDomainService
        {
        private readonly IWebhookRepository _webhookRepository;
        private ISmartNotification _notification;
        public HttpClient _httpClient = new HttpClient();
        ILogger<WebhookDomainService> _logger;
        public WebhookDomainService(
           IWebhookRepository webhookRepository,
           ISmartNotification notification,
           IUnitOfWork unitOfWork,
           INotificationHandler<DomainNotification> messageHandler,
           ILogger<WebhookDomainService> logger
       ) : base(webhookRepository, unitOfWork, messageHandler)
            {
            _webhookRepository = webhookRepository;
            _notification = notification;
            _logger = logger;
            }

        public async Task<string> GetFullPagseguroNotificationAsync(PagSeguroNotificationInput input)
            {
            ILogEventEnricher[] enrichers =
                {
                new PropertyEnricher("NotificationCode", input.NotificationCode),
                new PropertyEnricher("NotificationType", input.NotificationType),
                new PropertyEnricher("UrlBase", input.UrlBase)
                };

            using (LogContext.Push(enrichers))
                {
                _logger.LogInformation("GetFullPagseguroNotificationAsync initialized at {date} with parameter {@param}", DateTime.UtcNow, input);
                string fullPagseguroNotification = String.Empty;
                try
                    {
                    _httpClient.DefaultRequestHeaders.Accept.Clear();
                    string urlBaseToGetNotification = String.Concat(input.UrlBase, "/v3/transactions/notifications/", input.NotificationCode);
                    var uriToGetNotificationBuilder = new UriBuilder(urlBaseToGetNotification);
                    var queryString = HttpUtility.ParseQueryString(uriToGetNotificationBuilder.Query);
                    queryString["email"] = input.Email;
                    queryString["token"] = input.Token;
                    uriToGetNotificationBuilder.Query = queryString.ToString();

                    fullPagseguroNotification = await _httpClient.GetStringAsync(uriToGetNotificationBuilder.ToString());
                    }
                catch (Exception ex)
                    {
                    _logger.LogError("GetFullPagseguroNotificationAsync failed at {date} with error message {error}", DateTime.UtcNow, ex.Message);
                    }
                finally
                    {
                    _logger.LogInformation("GetFullPagseguroNotificationAsync executed at {date} with result {@param}", DateTime.UtcNow, fullPagseguroNotification);
                    }

                return fullPagseguroNotification;
                }
            }

        public async Task<string> GetPagseguroCheckoutCodeAsync(Dictionary<string, string> checkoutParams, PagSeguroNotificationInput input)
            {
            XmlDocument getFormUrlXMLResponse = new XmlDocument();
            string orderCode = String.Empty;
            ILogEventEnricher[] enrichers =
                {
                new PropertyEnricher("checkoutParams", checkoutParams),
                new PropertyEnricher("input", input)
                };

            using (LogContext.Push(enrichers))
                {
                _logger.LogInformation("GetPagseguroCheckoutCodeAsync initalized at {@date}", DateTime.UtcNow);
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                string urlBaseToGetNotification = String.Concat(input.UrlBase, "/v2/checkout");
                var uriToGetNotificationBuilder = new UriBuilder(urlBaseToGetNotification);
                var queryString = HttpUtility.ParseQueryString(uriToGetNotificationBuilder.Query);
                queryString["email"] = input.Email;
                queryString["token"] = input.Token;
                uriToGetNotificationBuilder.Query = queryString.ToString();
                var formUrlEncodedContent = new FormUrlEncodedContent(checkoutParams);

                try
                    {
                    var result = await _httpClient.PostAsync(uriToGetNotificationBuilder.ToString(), formUrlEncodedContent);
                    string resultContent = await result.Content.ReadAsStringAsync();
                    _logger.LogInformation("GetPagseguroCheckoutCodeAsync get as response to PostAsync to Pagseguro url {@pagSeguroUrl} with params {@formUrlEncodedContet} result {@resultContent} at {date}"
                        , uriToGetNotificationBuilder.ToString()
                        , formUrlEncodedContent, resultContent, DateTime.UtcNow);
                    getFormUrlXMLResponse.LoadXml(resultContent);
                    if (result.IsSuccessStatusCode)
                        {
                        orderCode = getFormUrlXMLResponse.ChildNodes[1].ChildNodes[0].InnerText;
                        }
                    }
                catch (Exception ex)
                    {
                    _logger.LogError("GetPagseguroCheckoutCodeAsync failed at {@date} with error message {@error}", DateTime.UtcNow, ex.Message);
                    }
                finally
                    {
                    _logger.LogInformation("GetPagseguroCheckoutCodeAsync executed at {@date}", DateTime.UtcNow);
                    }

                return orderCode;

                }
            }
        }
    }