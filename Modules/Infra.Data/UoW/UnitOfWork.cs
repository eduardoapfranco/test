using Domain.Interfaces.Repositories;
using Domain.Interfaces.UoW;
using Infra.CrossCutting.UoW.Models;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Infra.Data.UoW
{
    [ExcludeFromCodeCoverage]
    public class UnitOfWork : IUnitOfWork
    {
        readonly MySQLCoreContext _context;
        readonly SQLiteCoreContext _sqliteContext;
        public IServiceProvider _services { get; set; }

        public UnitOfWork(MySQLCoreContext context, SQLiteCoreContext sqliteContext, IServiceProvider serviceProvider)
        {
            var serviceCollection = new ServiceCollection();
            _services = serviceProvider;
            _context = context;
            _sqliteContext = sqliteContext;
        }

        public IDbContextTransaction DbTransaction { get; private set; }

        public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            return DbTransaction ?? (DbTransaction = _context.Database.BeginTransaction(isolationLevel));
        }

        public IDbContextTransaction BeginTransactionSQLite(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            return DbTransaction ?? (DbTransaction = _sqliteContext.Database.BeginTransaction(isolationLevel));
        }

        public CommandResponse Commit()
        {
            if (DbTransaction == null)
                return new CommandResponse(false);

            _context.SaveChanges();
            _context.Database.CurrentTransaction.Commit();

            DbTransaction = null;
            return new CommandResponse(true);
        }

        public CommandResponse CommitSQLite()
        {
            if (DbTransaction == null)
                return new CommandResponse(false);

            _sqliteContext.SaveChanges();
            _sqliteContext.Database.CurrentTransaction.Commit();

            DbTransaction = null;
            return new CommandResponse(true);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesSQLiteAsync()
        {
            await _sqliteContext.SaveChangesAsync();
        }

        public bool CurrentTransaction()
        {
            return _context.Database.CurrentTransaction == null ? false : true;
        }

        public bool CurrentTransactionSQLite()
        {
            return _sqliteContext.Database.CurrentTransaction == null ? false : true;
        }

        public IUserRepository User => _services.GetRequiredService<IUserRepository>();
        public IPlanRepository Plan => _services.GetRequiredService<IPlanRepository>();
        public IUserPlansRepository UserPlans => _services.GetRequiredService<IUserPlansRepository>();
        public IPasswordResetMobileRepository PasswordResetMobile => _services.GetRequiredService<IPasswordResetMobileRepository>();
        public ICategoryRepository Category => _services.GetRequiredService<ICategoryRepository>();
        public ICategorySQLiteRepository CategorySQLite => _services.GetRequiredService<ICategorySQLiteRepository>();
        public IChecklistRepository Checklist => _services.GetRequiredService<IChecklistRepository>();
        public IChecklistSQLiteRepository ChecklistSQLite => _services.GetRequiredService<IChecklistSQLiteRepository>();
        public IReminderRepository Reminder => _services.GetRequiredService<IReminderRepository>();
        public IPlanTypeRepository PlanType => _services.GetRequiredService<IPlanTypeRepository>();
        public IProfileRepository Profile => _services.GetRequiredService<IProfileRepository>();
        public IFunctionalityRepository Functionality => _services.GetRequiredService<IFunctionalityRepository>();
        public IProfileCategoryRepository ProfileCategory => _services.GetRequiredService<IProfileCategoryRepository>();
        public IProfileFunctionalityRepository ProfileFunctionality => _services.GetRequiredService<IProfileFunctionalityRepository>();
        public IRatingRepository Rating => _services.GetRequiredService<IRatingRepository>();
        public IContentSugestionRepository ContentSugestion => _services.GetRequiredService<IContentSugestionRepository>();
        public IConstructionRepository Construction => _services.GetRequiredService<IConstructionRepository>();
        public IUserPaymentMethodRepository UserPaymentMethod => _services.GetRequiredService<IUserPaymentMethodRepository>();

    }
}
