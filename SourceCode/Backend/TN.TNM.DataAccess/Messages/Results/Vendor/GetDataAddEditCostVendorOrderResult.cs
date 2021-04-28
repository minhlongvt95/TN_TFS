using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Cost;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetDataAddEditCostVendorOrderResult : BaseResult
    {
        public List<CostEntityModel> ListCost { get; set; }
    }
}
