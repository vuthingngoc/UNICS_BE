﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using UniCEC.Data.Enum;

namespace UniCEC.Data.ViewModels.Entities.Competition
{
    public class UpdateConstraintBeforePublishModel
    {
        public int Id { get; set; }

        public string? Content { get; set; }

        public CompetitionScopeStatus? Scope { get; set; }

        //Comment why you update
        public string Comment { get; set; }

        //---------Author to check user is Leader of Club and Collaborate in Copetition
        [JsonPropertyName("club_id")]
        public int ClubId { get; set; }
    }
}
