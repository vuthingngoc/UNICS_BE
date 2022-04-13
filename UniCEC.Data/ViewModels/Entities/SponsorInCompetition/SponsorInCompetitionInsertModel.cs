﻿using System.Text.Json.Serialization;

namespace UniCEC.Data.ViewModels.Entities.SponsorInCompetition
{
    public class SponsorInCompetitionInsertModel
    {
        public int CompetitionId { get; set; }
        [JsonPropertyName("sponsor_id")]
        public int SponsorId { get; set; }
    }
}