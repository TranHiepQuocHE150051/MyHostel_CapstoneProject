using System;
using System.Collections.Generic;

namespace MyHostel_Admin.Models
{
    public partial class Admin
    {
        public int Id { get; set; }
        public string AccountName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }

        public virtual Role Role { get; set; } = null!;
    }
}
