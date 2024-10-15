using Domain.Interfaces.Repositories;
using Infra.CrossCutting.UoW.Interfaces;

namespace Domain.Interfaces.UoW
{
    public interface IUnitOfWork : IUnitOfWorkBase
    {
        IUserRepository User { get; }
        IPlanRepository Plan { get; }
        IUserPlansRepository UserPlans { get; }
        IPasswordResetMobileRepository PasswordResetMobile { get; }
        ICategoryRepository Category { get; }
        IChecklistRepository Checklist { get; }
        ICategorySQLiteRepository CategorySQLite { get; }
        IChecklistSQLiteRepository ChecklistSQLite { get; }
        IReminderRepository Reminder { get; }
        IPlanTypeRepository PlanType { get; }
        IProfileRepository Profile { get; }
        IFunctionalityRepository Functionality { get; }
        IProfileCategoryRepository ProfileCategory { get; }
        IProfileFunctionalityRepository ProfileFunctionality { get; }    
        IRatingRepository Rating { get; }
        IContentSugestionRepository ContentSugestion { get; }
        IConstructionRepository Construction { get; }
        IUserPaymentMethodRepository UserPaymentMethod { get; }
    }
}
