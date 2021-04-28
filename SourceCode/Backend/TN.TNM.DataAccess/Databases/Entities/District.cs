using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class District
    {
        public District()
        {
            Contact = new HashSet<Contact>();
            Ward = new HashSet<Ward>();
        }

        public Guid DistrictId { get; set; }
        public Guid ProvinceId { get; set; }
        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }
        public string DistrictType { get; set; }
        public bool? Active { get; set; }

        public Province Province { get; set; }
        public ICollection<Contact> Contact { get; set; }
        public ICollection<Ward> Ward { get; set; }
    }
}
