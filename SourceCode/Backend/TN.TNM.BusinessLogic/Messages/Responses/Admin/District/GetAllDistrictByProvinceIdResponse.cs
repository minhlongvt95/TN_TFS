using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.District
{
    public class GetAllDistrictByProvinceIdResponse : BaseResponse
    {
        public List<DistrictModel> ListDistrict { get; set; }
    }
}
