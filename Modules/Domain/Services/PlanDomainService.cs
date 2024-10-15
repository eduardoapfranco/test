using Domain.Interfaces.Services;
using Domain.Interfaces.UoW;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Domain.Entities;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.Domain.Services;
using Domain.Interfaces.Repositories;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Domain.Services
{
    public class PlanDomainService : DomainService<Plan, int, IUnitOfWork>, IPlanDomainService
    {
        private readonly IPlanRepository _planRepository;
        private ISmartNotification _notification;
        private ILogger<PlanDomainService> _logger;
        public PlanDomainService(
           IPlanRepository planRepository,
           ISmartNotification notification,
           IUnitOfWork unitOfWork,
           INotificationHandler<DomainNotification> messageHandler,
           ILogger<PlanDomainService> logger
       ) : base(planRepository, unitOfWork, messageHandler)
        {
            _planRepository = planRepository;
            _notification = notification;
            _logger = logger;
        }

        public async Task<Plan> GetPremiumPlanAsync(int planId)
        {
            _logger.LogInformation($"Init {nameof(GetPremiumPlanAsync)} in {nameof(PlanDomainService)}for planId: {planId}");
            var premiumPlan = await _planRepository.GetWithType(planId);

            if (null == premiumPlan)
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(),
                    "Falha ao recuperar o plano premium!");
                _logger.LogWarning($"Fail {nameof(GetPremiumPlanAsync)} in {nameof(PlanDomainService)}for planId: {planId}");
                return default;
            }


            return premiumPlan;
        }

        public async Task<Plan> GetFreemiumPlanAsync()
        {
            var getFreemiumPlan = await _planRepository.SelectFilterAsync(x => x.Title.Equals("Freemium") && x.Active.Equals(1));
            _logger.LogInformation($"Init {nameof(GetFreemiumPlanAsync)} in {nameof(PlanDomainService)}");
            if (!getFreemiumPlan.Any())
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(),
                    "Falha ao recuperar o plano freemium!");
                _logger.LogInformation($"Fail {nameof(GetFreemiumPlanAsync)} in {nameof(PlanDomainService)}");

                return default;
            }
            return getFreemiumPlan.FirstOrDefault();
        }

        public async Task<IEnumerable<Plan>> GetPlansPremiumWithType()
        {
            return await _planRepository.GetPlansPremiumWithType();
        }

        public async Task<Plan> GetPlanByPartnerId(string partnerId)
            {
            var planId = partnerId.Replace("google:", "").Replace("apple:", "");
            var planList = await SelectFilterAsync(x => x.IdGoogle == planId || x.IdApple == planId);
            Plan plan = new Plan();
            if (planList.Any())
                {
                plan = planList.ToList().FirstOrDefault();
                return  await _planRepository.GetWithType(plan.Id);
                }
            else
                {
                _logger.LogInformation("GetPlanByPartnerId Plan with id {planId} not found at process fovea webhook at {date} with ", planId, System.DateTime.UtcNow);
                return null;
                }
            }
    }
}
