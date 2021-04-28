using System;

namespace TN.TNM.DataAccess.Models.Address
{
    public class WardEntityModel
    {
        public Guid WardId { get; set; }
        public Guid DistrictId { get; set; }
        public string WardName { get; set; }
        public string WardCode { get; set; }
        public string WardType { get; set; }
        public bool? Active { get; set; }
    }
}
