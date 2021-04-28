using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetAllProductCodeResult: BaseResult
    {
        public List<string> ListProductCode { get; set; }
    }
}
