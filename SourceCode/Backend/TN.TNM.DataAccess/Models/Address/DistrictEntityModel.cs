using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Address
{
    public class DistrictEntityModel
    {
        public Guid DistrictId { get; set; }
        public Guid ProvinceId { get; set; }
        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }
        public string DistrictType { get; set; }
        public bool? Active { get; set; }
        public List<WardEntityModel> WardList { get; set; }
    }
}
