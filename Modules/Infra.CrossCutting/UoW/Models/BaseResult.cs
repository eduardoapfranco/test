using FluentValidation.Results;
using Newtonsoft.Json;

namespace Infra.CrossCutting.UoW.Models
{
    public class BaseResult
    {
        [JsonIgnore]
        public ValidationResult ValidationResult { get; set; }
    }
}
