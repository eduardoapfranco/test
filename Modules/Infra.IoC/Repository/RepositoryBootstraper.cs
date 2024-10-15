using Domain.Interfaces.Repositories;
using Domain.Interfaces.UoW;
using Infra.Data.Context;
using Infra.Data.Repository;
using Infra.Data.UoW;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Infra.IoC.Repository
{
    [ExcludeFromCodeCoverage]
    internal class RepositoryBootstraper
    {
        internal void ChildServiceRegister(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MySQLCoreContext>(options => options.UseMySql(configuration.GetConnectionString("MySqlCore")));
            services.AddDbContext<SQLiteCoreContext>(options => options.UseSqlite(configuration.GetConnectionString("SqliteCore")));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPlanRepository, PlanRepository>();
            services.AddScoped<IUserPlansRepository, UserPlansRepository>();
            services.AddScoped<IPasswordResetMobileRepository, PasswordResetMobileRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategorySQLiteRepository, CategorySQLiteRepository>();
            services.AddScoped<IChecklistRepository, ChecklistRepository>();
            services.AddScoped<IChecklistSQLiteRepository, ChecklistSQLiteRepository>();
            services.AddScoped<IFileBlobStorageRepository, FileBlobStorageRepository>();
            services.AddScoped<IWebhookRepository, WebhookRepository>();
            services.AddScoped<IReminderRepository, ReminderRepository>();
            services.AddScoped<IPlanTypeRepository, PlanTypeRepository>();
            services.AddScoped<IFunctionalityRepository, FunctionalityRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IProfileCategoryRepository, ProfileCategoryRepository>();
            services.AddScoped<IProfileFunctionalityRepository, ProfileFunctionalityRepository>();
            services.AddScoped<IRatingRepository, RatingRepository>();
            services.AddScoped<IContentSugestionRepository, ContentSugestionRepository>();
            services.AddScoped<IVersionRepository, VersionRepository>();
            services.AddScoped<IConstructionRepository, ConstructionRepository>();
            services.AddScoped<IConstructionReportsRepository, ConstructionReportsRepository>();
            services.AddScoped<IUserPaymentMethodRepository, UserPaymentMethodRepository>();
            services.AddScoped<IAreaRepository, AreaRepository>();
            services.AddScoped<IUserAreaRepository, UserAreaRepository>();
            services.AddScoped<IConstructionReportsTypesRepository, ConstructionReportsTypesRepository>();
            }
        }
}
