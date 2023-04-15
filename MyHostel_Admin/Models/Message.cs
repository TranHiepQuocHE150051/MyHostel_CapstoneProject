using System;
using System.Collections.Generic;

namespace MyHostel_Admin.Models
{
    public partial class Message
    {
        public Message()
        {
            MessageImages = new HashSet<MessageImage>();
        }

        public int Id { get; set; }
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public string? MsgText { get; set; }
        public int? ParentMsgId { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte AnonymousFlg { get; set; }
        public int Status { get; set; }

        public virtual Chat Chat { get; set; } = null!;
        public virtual Member Sender { get; set; } = null!;
        public virtual ICollection<MessageImage> MessageImages { get; set; }
    }
}
