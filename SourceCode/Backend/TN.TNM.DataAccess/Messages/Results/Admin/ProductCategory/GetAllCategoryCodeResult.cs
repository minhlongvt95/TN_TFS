using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Admin.ProductCategory
{
    public class GetAllCategoryCodeResult : BaseResult
    {
        public List<string> ProductCategoryCodeList { get; set; }
    }
}
