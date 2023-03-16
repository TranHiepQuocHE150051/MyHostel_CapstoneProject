using System;
using System.Collections.Generic;

namespace MyHostel_BackEnd.Models
{
    public partial class Notification
    {
        public int Id { get; set; }
        public int SendTo { get; set; }
        public string? Message { get; set; }
        public byte[] SendAt { get; set; } = null!;

        public virtual Member SendToNavigation { get; set; } = null!;
    }
}
