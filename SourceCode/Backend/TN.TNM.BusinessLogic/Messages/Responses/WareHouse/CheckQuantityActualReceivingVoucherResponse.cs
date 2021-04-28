using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class CheckQuantityActualReceivingVoucherResponse:BaseResponse
    {
        public decimal SumTotalQuantityActual { get; set; }
        public bool IsEnough { get; set; }

    }
}
