using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Cost;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class CreateCostResult : BaseResult
    {
        public List<CostEntityModel> ListCost { get; set; }
    }
}
