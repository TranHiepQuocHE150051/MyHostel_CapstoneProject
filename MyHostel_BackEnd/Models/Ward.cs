﻿using System;
using System.Collections.Generic;

namespace MyHostel_BackEnd.Models
{
    public partial class Ward
    {
        public Ward()
        {
            Hostels = new HashSet<Hostel>();
        }

        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? NameEn { get; set; }
        public string? FullName { get; set; }
        public string? FullNameEn { get; set; }
        public string? CodeName { get; set; }
        public string? DistrictCode { get; set; }
        public int? AdministrativeUnitId { get; set; }

        public virtual AdministrativeUnit? AdministrativeUnit { get; set; }
        public virtual District? DistrictCodeNavigation { get; set; }
        public virtual ICollection<Hostel> Hostels { get; set; }
    }
}
