using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetProductByVendorIDResult : BaseResult
    {
        public List<Databases.Entities.Product> lstProduct { get; set; }
    }
}
