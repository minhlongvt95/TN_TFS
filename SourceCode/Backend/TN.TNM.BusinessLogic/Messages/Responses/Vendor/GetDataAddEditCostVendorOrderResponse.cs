using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Cost;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class GetDataAddEditCostVendorOrderResponse : BaseResponse
    {
        public List<CostModel> ListCost { get; set; }
    }
}
