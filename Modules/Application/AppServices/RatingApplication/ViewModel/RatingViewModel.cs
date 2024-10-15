﻿using Infra.CrossCutting.UoW.Models;
using System;
using Newtonsoft.Json;

namespace Application.AppServices.RatingApplication.ViewModel
    {
    public class RatingViewModel : BaseResult
    { 
        public long Id { get; set; }
        [JsonProperty("rating")]
        public int _Rating { get; set; }
        public string Comment { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        [JsonProperty("checklist_id")]
        public int? ChecklistId { get; set; }
        [JsonProperty("category_id")]
        public int? CategoryId { get; set; }
        [JsonProperty("user_Id")]
        public int UserId { get; set; }
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        }
}
