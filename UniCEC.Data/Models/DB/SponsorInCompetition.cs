﻿using System;
using System.Collections.Generic;

#nullable disable

namespace UniCEC.Data.Models.DB
{
    public partial class SponsorInCompetition
    {
        public int Id { get; set; }
        public int CompetitionId { get; set; }
        public int SponsorId { get; set; }
        public int UserId { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Comment { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string Feedback { get; set; }
        public int Status { get; set; }

        public virtual Competition Competition { get; set; }
        public virtual Sponsor Sponsor { get; set; }
    }
}
