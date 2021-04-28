using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetProductCategoryGroupByLevelResponse : BaseResponse
    {
        public List<dynamic> lstResult { get; set; }
    }
}
