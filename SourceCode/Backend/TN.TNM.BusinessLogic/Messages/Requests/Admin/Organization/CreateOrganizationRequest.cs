using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Organization;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization
{
    public class CreateOrganizationRequest : BaseRequest<CreateOrganizationParameter>
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

        public override CreateOrganizationParameter ToParameter()
        {
            return new CreateOrganizationParameter()
            {
                OrganizationName = OrganizationName,
                OrganizationCode = OrganizationCode,
                Address = Address,
                Phone = Phone,
                ParentId = ParentId,
                Level = Level,
                UserId = UserId,
                IsFinancialIndependence = IsFinancialIndependence,
                GeographicalAreaId = GeographicalAreaId,
                ProvinceId = ProvinceId,
                DistrictId = DistrictId,
                WardId = WardId,
                SatelliteId = SatelliteId,
                OrganizationOtherCode = OrganizationOtherCode
            };
        }
    }
}
