using System;
using System.Collections.Generic;

namespace MyHostel_BackEnd.Models
{
    public partial class Room
    {
        public Room()
        {
            Residents = new HashSet<Resident>();
        }

        public int Id { get; set; }
        public int HostelId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Resident> Residents { get; set; }
    }
}
