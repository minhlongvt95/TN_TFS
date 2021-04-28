using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class GetInventoryReceivingVoucherByIdParameter:BaseParameter
    {
        public Guid Id { get; set; }
    }
}
