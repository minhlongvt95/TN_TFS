using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class CheckQuantityActualReceivingVoucherParameter:BaseParameter
    {
        public Guid ObjectId { get; set; }
        public int Type { get; set; }
    }
}
