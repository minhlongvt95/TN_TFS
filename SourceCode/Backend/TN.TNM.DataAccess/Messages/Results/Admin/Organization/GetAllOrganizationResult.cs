using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.GeographicalArea;
using TN.TNM.DataAccess.Models.Satellite;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Organization
{
    public class GetAllOrganizationResult : BaseResult
    {
        public List<OrganizationEntityModel> OrganizationList { get; set; }
        public List<OrganizationEntityModel> ListAll { get; set; }
        public List<GeographicalAreaEntityModel> ListGeographicalArea { get; set; }
        public List<ProvinceEntityModel> ListProvince { get; set; }
        public List<DistrictEntityModel> ListDistrict { get; set; }
        public List<WardEntityModel> ListWard { get; set; }
        public List<SatelliteEntityModel> ListSatellite { get; set; }
    }
}
