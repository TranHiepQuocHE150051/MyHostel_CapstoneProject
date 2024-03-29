﻿using System;
using System.Collections.Generic;

namespace MyHostel_BackEnd.Models
{
    public partial class Room
    {
        public Room()
        {
            Residents = new HashSet<Resident>();
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public int HostelId { get; set; }
        public string Name { get; set; } = null!;
        public int? RoomArea { get; set; }
        public decimal? Price { get; set; }

        public virtual Hostel Hostel { get; set; } = null!;
        public virtual ICollection<Resident> Residents { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
