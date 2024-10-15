using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.Repositories;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class UserPlansRepository : GenericRepository<UserPlans, int, MySQLCoreContext>, IUserPlansRepository
    {
        public UserPlansRepository(MySQLCoreContext context, ILogger<GenericRepository<UserPlans, int, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }

        public async Task<UserPlans> GetPlanUserTerm(int userId)
        {
            var queryUserPlan = _context.UsersPlans.AsQueryable();
            var plansFreemium = await _context.Plans.Where(x => x.Title.Equals("Freemium")).Select(x => x.Id).ToListAsync();
            var plansTrial = await _context.Plans.Where(x => x.Title.Equals("Trial")).Select(x => x.Id).ToListAsync();
            var plansPremium = await _context.Plans.Where(x => x.Title.Equals("Premium")).Select(x => x.Id).ToListAsync();
            var userPlanEntity = new UserPlans();

            var query = from userPlan in queryUserPlan
                        where userPlan.UserId == userId
                        && userPlan.Deleted == (sbyte)BoolEnum.NO
                        &&
                            (
                                (
                                    userPlan.StatusPayment == (sbyte)BoolEnum.YES
                                    && plansPremium.Contains(userPlan.PlanId)
                                    && userPlan.CreatedAt.Date <= DateTime.Now.Date
                                    && userPlan.DueDateAt.Date >= DateTime.Now.Date
                                )
                                ||
                                (
                                    userPlan.StatusPayment == (sbyte)BoolEnum.YES
                                    && plansTrial.Contains(userPlan.PlanId)
                                    && userPlan.CreatedAt.Date <= DateTime.Now.Date
                                    && userPlan.DueDateAt.Date >= DateTime.Now.Date
                                )
                                ||
                                (
                                   userPlan.StatusPayment == (sbyte)BoolEnum.NO
                                   && plansFreemium.Contains(userPlan.PlanId)
                                ) 
                            )
                        select userPlan;
                        ;

            var resultPremium = await query.Where(x => x.PlanId > 1 && x.PlanId < 6).OrderByDescending(y => y.PlanId).FirstOrDefaultAsync();
            var result = await query.OrderByDescending(x => x.PlanId).FirstOrDefaultAsync();

            if (result == null)
            {
                return userPlanEntity.CreateDefaultFreemium();
            }
            if(resultPremium != null)
            {
            return resultPremium;
            }

            return result;
        }        
    }
}
