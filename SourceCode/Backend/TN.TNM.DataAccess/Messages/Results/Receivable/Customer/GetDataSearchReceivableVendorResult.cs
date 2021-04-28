using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Receivable.Customer
{
    public class GetDataSearchReceivableVendorResult : BaseResult
    {
        public List<Databases.Entities.Vendor> ListVendor { get; set; }
    }
}
