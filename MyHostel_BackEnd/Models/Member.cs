using System;
using System.Collections.Generic;

namespace MyHostel_BackEnd.Models
{
    public partial class Member
    {
        public Member()
        {
            Messages = new HashSet<Message>();
            Participants = new HashSet<Participant>();
            Residents = new HashSet<Resident>();
        }

        public int Id { get; set; }
        public string GoogleId { get; set; } = null!;
        public string FacebookId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Avatar { get; set; } = null!;
        public byte[] CreatedAt { get; set; } = null!;
        public int RoleId { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Participant> Participants { get; set; }
        public virtual ICollection<Resident> Residents { get; set; }
    }
}
