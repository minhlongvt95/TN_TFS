using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetProductAttributeByProductIDResult : BaseResult
    {
        public List<ProductAttributeCategory> lstProductAttributeCategory { get; set; }
    }
}
