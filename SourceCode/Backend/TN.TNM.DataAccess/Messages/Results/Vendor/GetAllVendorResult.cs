using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetAllVendorResult : BaseResult
    {
        public List<Databases.Entities.Vendor> VendorList { get; set; }
    }
}
