﻿using System.Text.Json.Serialization;

namespace UNICS.Data.ViewModels.Entities.ParticipantInTeam
{
    public class ViewParticipantInTeam
    {
        public int Id { get; set; }
        [JsonPropertyName("team_id")]
        public int TeamId { get; set; }
        [JsonPropertyName("participant_id")]
        public int ParticipantId { get; set; }
    }
}
