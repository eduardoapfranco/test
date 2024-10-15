using Infra.CrossCutting.UoW.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Threading.Tasks;

namespace Infra.CrossCutting.UoW.Interfaces
{
    public interface IUnitOfWorkBase
    {
        IDbContextTransaction DbTransaction { get; }
        IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        IDbContextTransaction BeginTransactionSQLite(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        CommandResponse Commit();
        CommandResponse CommitSQLite();
        bool CurrentTransaction();
        bool CurrentTransactionSQLite();
        Task SaveChangesAsync();
        Task SaveChangesSQLiteAsync();
    }
}
