using System;
using System.Collections.Generic;

namespace MyHostel_BackEnd.Models
{
    public partial class Participant
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int MemberId { get; set; }
        public byte[] JoinedAt { get; set; } = null!;
        public byte Role { get; set; }
        public int AnonymousTime { get; set; }

        public virtual Chat Chat { get; set; } = null!;
        public virtual Member Member { get; set; } = null!;
    }
}
