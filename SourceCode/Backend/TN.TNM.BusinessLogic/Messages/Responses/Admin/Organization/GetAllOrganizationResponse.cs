using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.GeographicalArea;
using TN.TNM.DataAccess.Models.Satellite;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Organization
{
    public class GetAllOrganizationResponse : BaseResponse
    {
        public List<OrganizationModel> OrganizationList { get; set; }
        public List<OrganizationModel> ListAll { get; set; }
        public List<GeographicalAreaEntityModel> ListGeographicalArea { get; set; }
        public List<ProvinceEntityModel> ListProvince { get; set; }
        public List<DistrictEntityModel> ListDistrict { get; set; }
        public List<WardEntityModel> ListWard { get; set; }
        public List<SatelliteEntityModel> ListSatellite { get; set; }
    }
}
