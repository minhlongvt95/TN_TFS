using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Product
{
    public class GetAllProductCodeResponse:BaseResponse
    {
        public List<string> ListProductCode { get; set; }
    }
}
