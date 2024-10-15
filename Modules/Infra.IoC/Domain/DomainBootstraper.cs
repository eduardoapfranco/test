using Domain.Core;
using Domain.Entities;
using Domain.Interfaces.Services;
using Domain.Services;
using Infra.CrossCutting.Auth.Intefaces;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Infra.IoC.Domain
{
    [ExcludeFromCodeCoverage]
    internal class DomainBootstraper
    {
        internal void ChildServiceRegister(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
            services.AddScoped<IAuthService<User>, AuthService>();
            services.AddScoped<IUserDomainService, UserDomainService>();
            services.AddScoped<IDbMobileDomainService, DbMobileDomainService>();
            services.AddScoped<ICategoryDomainService, CategoryDomainService>();
            services.AddScoped<IChecklistDomainService, ChecklistDomainService>();
            services.AddScoped<IWebhookDomainService, WebhookDomainService>();
            services.AddScoped<IPlanDomainService, PlanDomainService>();
            services.AddScoped<IReminderDomainService, ReminderDomainService>();
            services.AddScoped<IRatingDomainService, RatingDomainService>();
            services.AddScoped<IContentSugestionDomainService, ContentSugestionDomainService>();
            services.AddScoped<IVersionDomainService, VersionDomainService>();
            services.AddScoped<IConstructionDomainService, ConstructionDomainService>();
            services.AddScoped<IConstructionReportsDomainService, ConstructionReportsDomainService>();
            services.AddScoped<ISignatureDomainService, SignatureDomainService>();
            services.AddScoped<IUserPaymentMethodDomainService, UserPaymentMethodDomainService>();
            services.AddScoped<IRDStationDomainService, RDStationDomainService>();
            services.AddScoped<IAreaDomainService, AreaDomainService>();
            services.AddScoped<IUserAreaDomainService, UserAreaDomainService>();
            }
        }
}
