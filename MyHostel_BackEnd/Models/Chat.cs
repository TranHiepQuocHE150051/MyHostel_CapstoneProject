using System;
using System.Collections.Generic;

namespace MyHostel_BackEnd.Models
{
    public partial class Chat
    {
        public Chat()
        {
            Messages = new HashSet<Message>();
            Participants = new HashSet<Participant>();
        }

        public int Id { get; set; }
        public int HostelId { get; set; }
        public string Name { get; set; } = null!;
        public byte[] CreatedAt { get; set; } = null!;
        public DateTime LastMsgAt { get; set; }

        public virtual Hostel Hostel { get; set; } = null!;
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Participant> Participants { get; set; }
    }
}
