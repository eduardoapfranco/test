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
using System;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Serilog.Context;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Domain.Input.RDStation;
using Newtonsoft.Json;

namespace Domain.Services
    {
    public class RDStationDomainService : DomainService<User, int, IUnitOfWork>, IRDStationDomainService
    {
        private readonly IUserRepository _userRepository;
        private ISmartNotification _notification;
        public HttpClient _httpClient = new HttpClient();
        ILogger<RDStationDomainService> _logger;
        public RDStationDomainService(
           IUserRepository userRepository,
           ISmartNotification notification,
           IUnitOfWork unitOfWork,
           INotificationHandler<DomainNotification> messageHandler,
           ILogger<RDStationDomainService> logger
       ) : base(userRepository, unitOfWork, messageHandler)
        {
            _userRepository = userRepository;
            _notification = notification;
            _logger = logger;
        }

        public async Task<Conversion> PostConversionAsync(User user, RDStationInput input)
        {
            ILogEventEnricher[] enrichers =
                {
                new PropertyEnricher("UrlBase", input.UrlBase),
                new PropertyEnricher("Token", input.ApiSecret),
                new PropertyEnricher("UserEmail", user.Email),
                };

            using (LogContext.Push(enrichers))
            {
                _logger.LogInformation("PatchContactAsync initialized at {date} with  user {user} and parameter {@param}", DateTime.UtcNow, user, input);
                Conversion rdContact = new Conversion();
                rdContact.Payload.Name = user.Name;
                rdContact.Payload.Email = user.Email;
                string endpoint = String.Concat("/conversions");
                Conversion rdContactResponse = await Patch<Conversion>(rdContact, endpoint, input);
                return rdContactResponse;
            }
        }

        private async Task<T> Patch<T>(T objeto, string endpoint, RDStationInput input)
        {
            HttpResponseMessage postResult = new HttpResponseMessage();
            try
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                string urlPostRD = String.Concat(input.UrlBase, endpoint, "?api_key=", input.ApiSecret);
                var uriPostRD = new UriBuilder(urlPostRD);
                StringContent contentRequest = null;
                if (null != objeto)
                {
                    var jsonRequest = JsonConvert.SerializeObject(objeto);
                    contentRequest = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                }

                postResult = await _httpClient.PostAsync(uriPostRD.ToString(), contentRequest);
                string responseBody = await postResult.Content.ReadAsStringAsync();

                _logger.LogInformation("Patch executed at {date} with result {@param}", DateTime.UtcNow, postResult);
                return JsonConvert.DeserializeObject<T>(responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError("Patch failed at {date} with error message {error}", DateTime.UtcNow, ex.Message);
            }
            return default(T);
        }
    }
}
