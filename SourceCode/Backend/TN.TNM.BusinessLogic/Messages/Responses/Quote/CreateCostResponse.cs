using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Cost;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class CreateCostResponse : BaseResponse
    {
        public List<CostModel> ListCost { get; set; }
    }
}
