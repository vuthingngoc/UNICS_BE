﻿using System.Collections.Generic;

#nullable disable

namespace UniCEC.Data.Models.DB
{
    public partial class Major
    {
        public Major()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public string MajorCode { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
