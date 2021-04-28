using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Admin.District
{
    public class GetAllDistrictByProvinceIdResult : BaseResult
    {
        public List<Databases.Entities.District> ListDistrict { get; set; }
    }
}
