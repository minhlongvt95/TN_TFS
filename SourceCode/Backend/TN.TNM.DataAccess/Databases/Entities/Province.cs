using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Province
    {
        public Province()
        {
            Contact = new HashSet<Contact>();
            District = new HashSet<District>();
        }

        public Guid ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string ProvinceCode { get; set; }
        public string ProvinceType { get; set; }
        public bool? Active { get; set; }

        public ICollection<Contact> Contact { get; set; }
        public ICollection<District> District { get; set; }
    }
}
