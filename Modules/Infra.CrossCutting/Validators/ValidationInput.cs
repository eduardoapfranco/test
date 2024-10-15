using FluentValidation.Results;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Infra.CrossCutting.Validators
{
    [ExcludeFromCodeCoverage]
    public abstract class ValidationInput
    {
        [JsonIgnore]
        [NotMapped]
        public ValidationResult ValidationResult { get; set; }

        public virtual bool IsValid()
        {
            return true;
        }
    }
}
