﻿using System;
using System.Text.Json.Serialization;

namespace UNICS.Data.ViewModels.Entities.ClubPrevious
{
    public class ViewClubPrevious
    {
        public int Id { get; set; }
        [JsonPropertyName("club_role_id")]
        public int ClubRoleId { get; set; }
        [JsonPropertyName("club_id")]
        public int ClubId { get; set; }
        [JsonPropertyName("member_id")]
        public int MemberId { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }
        [JsonPropertyName("end_time")]
        public DateTime? EndTime { get; set; }
    }
}
