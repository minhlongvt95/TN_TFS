using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Ward
    {
        public Ward()
        {
            Contact = new HashSet<Contact>();
        }

        public Guid WardId { get; set; }
        public Guid DistrictId { get; set; }
        public string WardName { get; set; }
        public string WardCode { get; set; }
        public string WardType { get; set; }
        public bool? Active { get; set; }

        public District District { get; set; }
        public ICollection<Contact> Contact { get; set; }
    }
}
