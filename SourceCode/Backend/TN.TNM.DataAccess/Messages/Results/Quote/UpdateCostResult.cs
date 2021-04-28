using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Cost;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class UpdateCostResult : BaseResult
    {
        public List<CostEntityModel> ListCost { get; set; }
    }
}
