using System;
using System.Collections.Generic;

namespace MyHostel_BackEnd.Models
{
    public partial class Member
    {
        public Member()
        {
            Hostels = new HashSet<Hostel>();
            Messages = new HashSet<Message>();
            Notifications = new HashSet<Notification>();
            Participants = new HashSet<Participant>();
            Residents = new HashSet<Resident>();
        }

        public int Id { get; set; }
        public string? GoogleId { get; set; }
        public string? FacebookId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Avatar { get; set; } = null!;
        public byte[] CreatedAt { get; set; } = null!;
        public int RoleId { get; set; }
        public string FcmToken { get; set; } = null!;

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Hostel> Hostels { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Participant> Participants { get; set; }
        public virtual ICollection<Resident> Residents { get; set; }
    }
}
