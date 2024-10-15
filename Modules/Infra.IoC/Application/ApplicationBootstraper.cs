using Application.AppServices.CategoryApplication;
using Application.AppServices.ChecklistApplication;
using Application.AppServices.DbMobileApplication;
using Application.AppServices.ReminderApplication;
using Application.AppServices.UserApplication;
using Application.AppServices.OrderApplication;
using Application.Interfaces;
using Infra.CrossCutting.BlobStorage;
using Infra.CrossCutting.BlobStorage.Interfaces;
using Infra.CrossCutting.Email.Interfaces;
using Infra.CrossCutting.Email.Services;
using Infra.CrossCutting.Notification.Implements;
using Infra.CrossCutting.Notification.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Application.AppServices.RatingApplication;
using Application.AppServices.ContentSugestionApplication;
using Infra.CrossCutting.PDF.Interfaces;
using Infra.CrossCutting.PDF;
using Application.AppServices.VersionApplication;
using Application.AppServices.ConstructionApplication;
using Application.AppServices.SignatureApplication;
using Application.AppServices.RDApplication;
using Application.AppServices.ConstructionReportApplication;

namespace Infra.IoC.Application
{
    [ExcludeFromCodeCoverage]
    internal class ApplicationBootstraper
    {
        internal void ChildServiceRegister(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IBlob>(provider => new BlobAzure(configuration.GetConnectionString("AzureBlob")));
            services.AddScoped<ISmartNotification, SmartNotification>();
            services.AddScoped<IUserApplication, UserApplication>();
            services.AddScoped<IEmailSendService, EmailSendService>();
            services.AddScoped<IDbMobileApplication, DbMobileApplication>();
            services.AddScoped<ICategoryApplication, CategoryApplication>();
            services.AddScoped<IChecklistApplication, ChecklistApplication>();
            services.AddScoped<IOrderApplication, OrderApplication>();
            services.AddScoped<IExportPDF, ExportPdf>();
            services.AddScoped<IReminderApplication, ReminderApplication>();
            services.AddScoped<IRatingApplication, RatingApplication>();
            services.AddScoped<IContentSugestionApplication, ContentSugestionApplication>();
            services.AddScoped<IConstructionApplication, ConstructionApplication>();
            services.AddScoped<IConstructionReportApplication, ConstructionReportApplication>();
            services.AddScoped<ISignatureApplication, SignatureApplication>();
            services.AddScoped<IRDApplication, RDStationApplication>();
            }
        }
}
