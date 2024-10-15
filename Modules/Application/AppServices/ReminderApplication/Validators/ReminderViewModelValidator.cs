using Application.AppServices.ReminderApplication.ViewModel;
using FluentValidation;


namespace Application.AppServices.ReminderApplication.Validators
    {
    public static class ReminderViewModelErrorMessages
        {
        public static string TituloRequired = "Título é obrigatório";
        public static string StartTimeRequired = "Data/Hora de início é obrigatória";
        public static string EndTimeRequired = "Data/Hora de fim é obrigatória";
        public static string AppIdRequired = "Identificador é obrigatório";
        public static string InvalidId = "Identificador inválido";
        public static string InvalidAppId = "Identificador lógico inválido";
        }

    class ReminderViewModelValidatorInsert : AbstractValidator<ReminderViewModel>
        {
        public ReminderViewModelValidatorInsert()
            {
            RuleFor(doc => doc.Title).NotEmpty().OverridePropertyName(ReminderViewModelErrorMessages.TituloRequired);
            RuleFor(doc => doc.StartTime).NotNull().OverridePropertyName(ReminderViewModelErrorMessages.StartTimeRequired);
            RuleFor(doc => doc.EndTime).NotNull().OverridePropertyName(ReminderViewModelErrorMessages.EndTimeRequired);
            }
        }

    class ReminderViewModelValidatorUpdate : AbstractValidator<ReminderViewModel>
        {
        public ReminderViewModelValidatorUpdate()
            {
            RuleFor(doc => doc.Title).NotEmpty().OverridePropertyName(ReminderViewModelErrorMessages.TituloRequired);
            RuleFor(doc => doc.StartTime).NotNull().OverridePropertyName(ReminderViewModelErrorMessages.StartTimeRequired);
            RuleFor(doc => doc.EndTime).NotNull().OverridePropertyName(ReminderViewModelErrorMessages.EndTimeRequired);
            RuleFor(doc => doc.AppId).NotEmpty().OverridePropertyName(ReminderViewModelErrorMessages.AppIdRequired);
            }
        }

    class ReminderViewModelValidatorDelete : AbstractValidator<ReminderViewModel>
        {
        public ReminderViewModelValidatorDelete()
            {
            RuleFor(doc => doc.AppId).NotEmpty().OverridePropertyName(ReminderViewModelErrorMessages.AppIdRequired);
            }
        }

    class ReminderViewModelValidatorLogicalDelete : AbstractValidator<ReminderViewModel>
        {
        public ReminderViewModelValidatorLogicalDelete()
            {
            RuleFor(doc => doc.AppId).NotEmpty().OverridePropertyName(ReminderViewModelErrorMessages.InvalidAppId);
            }
        }
    }
