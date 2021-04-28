using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Address;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Province
{
    public class GetAllProvinceResult : BaseResult
    {
        public List<ProvinceEntityModel> ListProvince { get; set; }
    }
}
