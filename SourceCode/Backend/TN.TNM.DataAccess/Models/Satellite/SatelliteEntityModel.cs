using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Satellite
{
    public class SatelliteEntityModel
    {
        public Guid SatelliteId { get; set; }
        public string SatelliteCode { get; set; }
        public string SatelliteName { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
