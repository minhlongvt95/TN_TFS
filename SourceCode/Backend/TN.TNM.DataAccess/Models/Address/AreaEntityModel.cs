using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Address
{
    public class AreaEntityModel
    {
        public Guid AreaId { get; set; }
        public string AreaName { get; set; }
        public string AreaCode { get; set; }
        public bool? Active { get; set; }
    }
}
