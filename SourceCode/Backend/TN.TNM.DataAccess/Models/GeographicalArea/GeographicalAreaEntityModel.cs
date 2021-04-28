using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.GeographicalArea
{
    public class GeographicalAreaEntityModel
    {
        public Guid GeographicalAreaId { get; set; }
        public string GeographicalAreaCode { get; set; }
        public string GeographicalAreaName { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
