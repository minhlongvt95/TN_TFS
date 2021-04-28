using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetProductCategoryGroupByManagerResponse : BaseResponse
    {
        public List<dynamic> lstResult { get; set; }
    }
}
