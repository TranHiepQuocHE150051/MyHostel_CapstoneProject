using System;
using System.Collections.Generic;

namespace MyHostel_BackEnd.Models
{
    public partial class Message
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public string MsgText { get; set; } = null!;
        public int ParentMsgId { get; set; }
        public byte[] CreatedAt { get; set; } = null!;
        public bool AnonymousFlg { get; set; }

        public virtual Chat Chat { get; set; } = null!;
        public virtual Member Sender { get; set; } = null!;
    }
}
