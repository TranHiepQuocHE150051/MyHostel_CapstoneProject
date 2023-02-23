﻿using System;
using System.Collections.Generic;

namespace MyHostel_BackEnd.Models
{
    public partial class Amenity
    {
        public Amenity()
        {
            Hosteds = new HashSet<Hostel>();
        }

        public int Id { get; set; }
        public string AmenitiyName { get; set; } = null!;

        public virtual ICollection<Hostel> Hosteds { get; set; }
    }
}
