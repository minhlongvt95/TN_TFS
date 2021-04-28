using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class CheckQuantityActualReceivingVoucherResult:BaseResult
    {
        public decimal SumTotalQuantityActual { get; set; }
        public bool IsEnough { get; set; }
    }
}
