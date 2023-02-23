using System;
using System.Collections.Generic;

namespace MyHostel_BackEnd.Models
{
    public partial class Role
    {
        public Role()
        {
            Admins = new HashSet<Admin>();
            Members = new HashSet<Member>();
        }

        public int Id { get; set; }
        public string RoleName { get; set; } = null!;

        public virtual ICollection<Admin> Admins { get; set; }
        public virtual ICollection<Member> Members { get; set; }
    }
}
