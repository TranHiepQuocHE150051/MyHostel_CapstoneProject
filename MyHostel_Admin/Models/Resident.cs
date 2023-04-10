using System;
using System.Collections.Generic;

namespace MyHostel_Admin.Models
{
    public partial class Resident
    {
        public int HostelId { get; set; }
        public int MemberId { get; set; }
        public int RoomId { get; set; }
        public byte Status { get; set; }
        public byte Rate { get; set; }
        public string Comment { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int Id { get; set; }
        public DateTime? LeftAt { get; set; }

        public virtual Hostel Hostel { get; set; } = null!;
        public virtual Member Member { get; set; } = null!;
        public virtual Room Room { get; set; } = null!;
    }
}
