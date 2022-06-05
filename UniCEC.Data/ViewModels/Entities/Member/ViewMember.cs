﻿using System;
using System.Text.Json.Serialization;
using UniCEC.Data.Enum;

namespace UniCEC.Data.ViewModels.Entities.Member
{
    public class ViewMember
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("club_role_id")]
        public int ClubRoleId { get; set; }
        [JsonPropertyName("club_role_name")]
        public string ClubRoleName { get; set; }
        public string Avatar { get; set; }
        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }
        [JsonPropertyName("end_time")]
        public DateTime? EndTime { get; set; }
        [JsonPropertyName("term_id")]
        public int TermId { get; set; }
        [JsonPropertyName("term_name")]
        public string TermName { get; set; }
        public MemberStatus Status { get; set; }
        [JsonPropertyName("is_online")]
        public bool IsOnline { get; set; }
    }
}
