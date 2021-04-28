using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Province
{
    public class GetAllProvinceResponse : BaseResponse
    {
        public List<ProvinceModel> ListProvince { get; set; }
    }
}
