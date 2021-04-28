using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class CheckQuantityActualReceivingVoucherRequest : BaseRequest<CheckQuantityActualReceivingVoucherParameter>
    {
        public Guid ObjectId { get; set; }
        public int Type { get; set; }

        public override CheckQuantityActualReceivingVoucherParameter ToParameter()
        {
            return new CheckQuantityActualReceivingVoucherParameter
            {
                ObjectId = ObjectId,
                Type = Type
            };
        }
    }
}
