using System;
using System.Collections.Generic;

namespace MyHostel_BackEnd.Models
{
    public partial class Notification
    {
        public int Id { get; set; }
        public int SendTo { get; set; }
        public string? Message { get; set; }
        public DateTime SendAt { get; set; }
        public bool? Type { get; set; }
        public int? SendAtHour { get; set; }
        public DateTime? CreateAt { get; set; }

        public virtual Member SendToNavigation { get; set; } = null!;
    }
}
