using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.Repositories;
using Domain.Utils;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class PlanRepository : GenericRepository<Plan, int, MySQLCoreContext>, IPlanRepository
    {
        public PlanRepository(MySQLCoreContext context, ILogger<GenericRepository<Plan, int, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }

        public async Task<Plan> GetWithType(int planId)
        {
            var queryPlans = _context.Plans.AsQueryable();
            var planEntity = new Plan();
            var query = from plan in queryPlans
                        join planType in _context.PlansTypes on (int) plan.Type equals planType.Id
                        where plan.Id == planId && plan.Active == (byte) BoolEnum.YES                        
                        select new Plan()
                        {
                            Id = plan.Id,
                            IdGoogle = plan.IdGoogle,
                            IdApple = plan.IdApple,
                            Title = plan.Title,
                            Content = plan.Content,
                            Active = plan.Active,
                            CreatedAt = plan.CreatedAt,
                            Type = plan.Type,
                            UpdatedAt = plan.UpdatedAt,
                            Value = plan.Value,
                            ValueSave = plan.ValueSave,
                            ValueFinally = plan.ValueFinally,
                            PlanType = planType
                        };
            

            var result = await query.FirstOrDefaultAsync();

            if (result == null)
            {
                return CreateDefaultFreemium.CreatePlan();
            }

            return result;
        }

        public async Task<Plan> GetWithTypeByTitle(string title)
            {
            var queryPlans = _context.Plans.AsQueryable();
            var planEntity = new Plan();
            var query = from plan in queryPlans
                        join planType in _context.PlansTypes on (int)plan.Type equals planType.Id
                        where plan.Title == title && plan.Active == (byte)BoolEnum.YES
                        select new Plan()
                            {
                            Id = plan.Id,
                            IdGoogle = plan.IdGoogle,
                            IdApple = plan.IdApple,
                            Title = plan.Title,
                            Content = plan.Content,
                            Active = plan.Active,
                            CreatedAt = plan.CreatedAt,
                            Type = plan.Type,
                            UpdatedAt = plan.UpdatedAt,
                            Value = plan.Value,
                            ValueSave = plan.ValueSave,
                            ValueFinally = plan.ValueFinally,
                            PlanType = planType
                            };


            var result = await query.FirstOrDefaultAsync();

            return result;
            }

        public async Task<IEnumerable<Plan>> GetPlansPremiumWithType()
        {
            var queryPlans = _context.Plans.AsQueryable();
            var planEntity = new Plan();
            var query = from plan in queryPlans
                        join planType in _context.PlansTypes on (int)plan.Type equals planType.Id
                        where plan.Active == (byte)BoolEnum.YES && plan.Title.Equals("Premium")
                        select new Plan()
                        {
                            Id = plan.Id,
                            IdGoogle = plan.IdGoogle,
                            IdApple = plan.IdApple,
                            Title = plan.Title,
                            Content = plan.Content,
                            Active = plan.Active,
                            CreatedAt = plan.CreatedAt,
                            Type = plan.Type,
                            UpdatedAt = plan.UpdatedAt,
                            Value = plan.Value,
                            ValueSave = plan.ValueSave,
                            ValueFinally = plan.ValueFinally,
                            PlanType = planType
                        };

            return await query.OrderBy(x => x.Type).ToListAsync();           
        }
    }
}
