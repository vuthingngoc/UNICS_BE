﻿using System;
using System.Collections.Generic;

#nullable disable

namespace UniCEC.Data.Models.DB
{
    public partial class ClubRole
    {
        public ClubRole()
        {
            ClubHistories = new HashSet<ClubHistory>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ClubHistory> ClubHistories { get; set; }
    }
}
