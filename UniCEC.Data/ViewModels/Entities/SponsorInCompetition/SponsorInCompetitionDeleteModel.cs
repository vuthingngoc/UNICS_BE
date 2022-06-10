﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UniCEC.Data.ViewModels.Entities.SponsorInCompetition
{
    public class SponsorInCompetitionDeleteModel
    {
        [JsonPropertyName("competition_id")]
        public int CompetitionId { get; set; }

        [JsonPropertyName("sponsor_id")]
        public int SponsorId { get; set; }

        [JsonPropertyName("club_id")]
        public int ClubId { get; set; }

    }
}
