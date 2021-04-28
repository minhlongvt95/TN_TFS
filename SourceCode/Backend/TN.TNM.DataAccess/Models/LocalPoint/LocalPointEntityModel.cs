using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.LocalPoint
{
    public class LocalPointEntityModel
    {
        public Guid LocalPointId { get; set; }
        public string LocalPointCode { get; set; }
        public string LocalPointName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public int StatusId { get; set; }
        public Guid LocalAddressId { get; set; }

        public string StatusName { get; set; }
    }
}
