﻿using System;
using System.Collections.Generic;

#nullable disable

namespace UniCEC.Data.Models.DB
{
    public partial class ParticipantInTeam
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int ParticipantId { get; set; }
        public int Status { get; set; }
        public int TeamRoleId { get; set; }

        public virtual Participant Participant { get; set; }
        public virtual Team Team { get; set; }
        public virtual TeamRole TeamRole { get; set; }
    }
}
