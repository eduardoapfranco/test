using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Input.RDStation;
using Domain.Interfaces.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Application.AppServices.RDApplication
    {
    public class RDStationApplication : BaseValidationService, IRDApplication
        {
        private readonly IRDStationDomainService _rdStationDomainService;
        private readonly ISmartNotification _notification;
        private readonly IMapper _mapper;
        ILogger<RDStationApplication> _logger;


        public RDStationApplication(IRDStationDomainService rdStationDomainService, 
            IMapper mapper, 
            ISmartNotification notification,
            ILogger<RDStationApplication> logger) : base(notification)
            {
            _rdStationDomainService = rdStationDomainService;
            _notification = notification;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<string>> CreateRDStationConversion(RDStationInput input)
            {
            var baseDate = DateTime.ParseExact("01/08/2021", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var users = await _rdStationDomainService.SelectFilterAsync(x => x.CreatedAt >= baseDate && String.IsNullOrEmpty(x.RDConversionID));
            var listOfEventsUuids = new List<string>();
            if (!users.Any())
                {
                _logger.LogInformation($"{nameof(CreateRDStationConversion)} Users not found to send conversion to RDStation at: {DateTime.Now}");
                return default;
                }
            foreach (User user in users)
                {
                _logger.LogInformation($"{nameof(CreateRDStationConversion)} Send conversion of user with e-mail {user.Email} to RDStation at: {DateTime.Now}");
                Conversion rdContact = await _rdStationDomainService.PostConversionAsync(user, input);
                user.RDConversionID = rdContact.EventUuid;
                await _rdStationDomainService.UpdateAsync(user);
                listOfEventsUuids.Add(rdContact.EventUuid);
                }
            return listOfEventsUuids;
            }
        }
}
