using Domain.Entities;
using Infra.Data.Config;
using Infra.Data.Config.Mappers.MySQLCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.Data.Context
{
    [ExcludeFromCodeCoverage]
    public class MySQLCoreContext : DbContext
    {
        public MySQLCoreContext(DbContextOptions<MySQLCoreContext> options) : base(options)
        {}

        public DbSet<User> Users { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<PlanType> PlansTypes { get; set; }
        public DbSet<UserPlans> UsersPlans { get; set; }
        public DbSet<PasswordReset> PasswordsResetMobile { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Checklist> Checklists { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<ContentSugestion> ContentSugestions { get; set; }
        public DbSet<Version> Versions { get; set; }
        public DbSet<FileBlobStorage> FilesBlobStorage { get; set; }
        public DbSet<Functionality> Functionalities { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<ProfileCategory> ProfilesCategories { get; set; }
        public DbSet<ProfileFunctionality> ProfilesFunctionalities { get; set; }

        public DbSet<Construction> Constructions { get; set; }
        public DbSet<UserPaymentMethod> UserPaymentMethods { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<UserAreas> UserAreas { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfiguration(new UserMapMySQLCoreConfig())
                .ApplyConfiguration(new PlanMapMySQLCoreConfig())
                .ApplyConfiguration(new UserPlansMapMySQLCoreConfig())
                .ApplyConfiguration(new PasswordResetMobileMapMySQLCoreConfig())
                .ApplyConfiguration(new CategoryMapMySQLCoreConfig())
                .ApplyConfiguration(new ChecklistMapMySQLCoreConfig())
                .ApplyConfiguration(new FileBlobStorageMapMySQLCoreConfig())
                .ApplyConfiguration(new WebhookMapMySQLCoreConfig())
                .ApplyConfiguration(new ReminderMapMySQLCoreConfig()) 
                .ApplyConfiguration(new PlanTypeMapMySQLCoreConfig())
                .ApplyConfiguration(new ProfileMapMySQLCoreConfig())
                .ApplyConfiguration(new FunctionalityMapMySQLCoreConfig())
                .ApplyConfiguration(new ProfileCategoryMapMySQLCoreConfig())
                .ApplyConfiguration(new ProfileFunctionalityMapMySQLCoreConfig())
                .ApplyConfiguration(new RatingMapMySQLCoreConfig())
                .ApplyConfiguration(new ContentSugestionMapMySQLCoreConfig())
                .ApplyConfiguration(new ConstructionMapMySQLCoreConfig())
                .ApplyConfiguration(new ConstructionReportsMapMySQLCoreConfig())
                .ApplyConfiguration(new VersionMapMySQLCoreConfig())
                .ApplyConfiguration(new UserPaymentMethodMapMySQLCoreConfig())
                .ApplyConfiguration(new AreaMapMySQLCoreConfig())
                .ApplyConfiguration(new UserAreasMapMySQLCoreConfig())
                .ApplyConfiguration(new ConstructionReportsTypesMapMySQLCoreConfig())
                ;
            }

        public override int SaveChanges()
        {
            ConfigPropertieDefault.SaveDefaultPropertiesChanges(ChangeTracker);
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            ConfigPropertieDefault.SaveDefaultPropertiesChanges(ChangeTracker);
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
