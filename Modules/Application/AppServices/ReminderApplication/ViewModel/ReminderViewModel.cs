using Application.AppServices.ReminderApplication.Validators;
using Infra.CrossCutting.UoW.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;


namespace Application.AppServices.ReminderApplication.ViewModel
    {
    public class ReminderViewModel : BaseResult
        {
        [JsonIgnoreAttribute]
        public long Id { get; set; }

        [JsonProperty("id")]
        public string AppId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("startTime")]
        public DateTime? StartTime { get; set; }
        [JsonProperty("endTime")]
        public DateTime? EndTime { get; set; }
        [JsonProperty("lastUpdated")]
        public DateTime? LastUpdated { get; set; }
        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        public bool IsEquals(ReminderViewModel that)
            {
            return this.AppId == that.AppId &&
                    this.Title == that.Title &&
                    this.Description == that.Description &&
                    this.StartTime == that.StartTime &&
                    this.EndTime == that.EndTime;
            }

        public bool IsNewer(ReminderViewModel that)
            {
            if (!this.LastUpdated.HasValue || !that.LastUpdated.HasValue)
                return true;

            return this.LastUpdated.Value > that.LastUpdated.Value;
            }

        public bool IsValidForInsert()
            {
            ValidationResult = new ReminderViewModelValidatorInsert().Validate(this);
            return ValidationResult.IsValid;
            }

        public bool IsValidForUpdate()
            {
            ValidationResult = new ReminderViewModelValidatorUpdate().Validate(this);
            return ValidationResult.IsValid;
            }

        public bool IsValidForDelete()
            {
            ValidationResult = new ReminderViewModelValidatorDelete().Validate(this);
            return ValidationResult.IsValid;
            }

        public bool IsValidForLogicalDelete()
            {
            ValidationResult = new ReminderViewModelValidatorLogicalDelete().Validate(this);
            return ValidationResult.IsValid;
            }
        }
    }
