﻿using System;
using System.Collections.Generic;

namespace MyHostel_Admin.Models
{
    public partial class District
    {
        public District()
        {
            Wards = new HashSet<Ward>();
        }

        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? NameEn { get; set; }
        public string? FullName { get; set; }
        public string? FullNameEn { get; set; }
        public string? CodeName { get; set; }
        public string? ProvinceCode { get; set; }
        public int? AdministrativeUnitId { get; set; }

        public virtual AdministrativeUnit? AdministrativeUnit { get; set; }
        public virtual Province? ProvinceCodeNavigation { get; set; }
        public virtual ICollection<Ward> Wards { get; set; }
    }
}
