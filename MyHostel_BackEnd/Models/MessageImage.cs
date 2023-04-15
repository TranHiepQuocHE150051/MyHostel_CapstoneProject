using System;
using System.Collections.Generic;

namespace MyHostel_BackEnd.Models
{
    public partial class MessageImage
    {
        public int MessageId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int Id { get; set; }

        public virtual Message Message { get; set; } = null!;
    }
}
