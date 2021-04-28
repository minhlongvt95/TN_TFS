using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetProductCategoryGroupByLevelResult : BaseResult
    {
        public List<dynamic> lstResult { get; set; }
    }
}
