using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Address
{
    public class ProvinceEntityModel
    {
        public Guid ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string ProvinceCode { get; set; }
        public string ProvinceType { get; set; }
        public bool? Active { get; set; }
        public List<DistrictEntityModel> DistrictList { get; set; }
    }
}
