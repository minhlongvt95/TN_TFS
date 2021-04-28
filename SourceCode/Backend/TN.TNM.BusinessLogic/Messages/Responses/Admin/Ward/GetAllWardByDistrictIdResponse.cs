using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Ward
{
    public class GetAllWardByDistrictIdResponse : BaseResponse
    {
        public List<WardModel> ListWard { get; set; }
    }
}
