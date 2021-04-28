using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Organization
{
    public class CreateOrganizationParameter : BaseParameter
    {
        public string OrganizationName { get; set; }
        public string OrganizationCode { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int Level { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsFinancialIndependence { get; set; }
        public Guid? GeographicalAreaId { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? WardId { get; set; }
        public Guid? SatelliteId { get; set; }
        public string OrganizationOtherCode { get; set; }
    }
}
