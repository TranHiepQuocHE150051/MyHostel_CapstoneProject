using System;
using System.Collections.Generic;

namespace MyHostel_Admin.Models
{
    public partial class Transaction
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public decimal? Rent { get; set; }
        public decimal? Electricity { get; set; }
        public decimal? Water { get; set; }
        public decimal? Internet { get; set; }
        public string? Other { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? AtTime { get; set; }

        public virtual Room Room { get; set; } = null!;
    }
}
