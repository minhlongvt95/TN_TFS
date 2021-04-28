using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Responses.CustomerCare
{
    public class GetTotalInteractiveResponse : BaseResponse
    {
        public List<GetTotalInteractiveModel> ListCate { get; set; }
        public int TotalCare { get; set; }
    }
}
