﻿using System;
using System.Text.Json.Serialization;

namespace UNICS.Data.ViewModels.Entities.University
{
    public class ViewUniversity
    {
        public int Id { get; set; }
        [JsonPropertyName("city_id")]
        public int CityId { get; set; }
        public string UniCode { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public DateTime Founding { get; set; }
        public string Openning { get; set; }
        public string Closing { get; set; }
        public bool Status { get; set; }
    }
}
