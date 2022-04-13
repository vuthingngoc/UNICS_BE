﻿using System;
using System.Text.Json.Serialization;

namespace UniCEC.Data.ViewModels.Entities.CompetitionEntity
{
    public class CompetitionEntityUpdateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Description { get; set; }
        [JsonPropertyName("last_modified")]
        public DateTime LastModified { get; set; }
    }
}