using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.LocalPoint;

namespace TN.TNM.DataAccess.Models.LocalAddress
{
    public class LocalAddressEntityModel
    {
        public Guid LocalAddressId { get; set; }
        public string LocalAddressCode { get; set; }
        public string LocalAddressName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public Guid BranchId { get; set; }

        public List<LocalPointEntityModel> ListLocalPoint { get; set; }
    }
}
