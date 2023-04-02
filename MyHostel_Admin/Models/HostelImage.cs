using System;
using System.Collections.Generic;

namespace MyHostel_Admin.Models
{
    public partial class HostelImage
    {
        public int HostelId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int Id { get; set; }

        public virtual Hostel Hostel { get; set; } = null!;
    }
}
