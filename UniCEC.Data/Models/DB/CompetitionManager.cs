﻿using System;
using System.Collections.Generic;

#nullable disable

namespace UniCEC.Data.Models.DB
{
    public partial class CompetitionManager
    {
        public int Id { get; set; }
        public int CompetitionInClubId { get; set; }
        public int CompetitionRoleId { get; set; }
        public int UserId { get; set; }
        public bool Status { get; set; }

        public virtual CompetitionInClub CompetitionInClub { get; set; }
        public virtual CompetitionRole CompetitionRole { get; set; }
    }
}
