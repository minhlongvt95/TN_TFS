using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetInventoryReceivingVoucherByIdRequest : BaseRequest<GetInventoryReceivingVoucherByIdParameter>
    {
        public Guid Id { get; set; }

        public override GetInventoryReceivingVoucherByIdParameter ToParameter()
        {
            return new GetInventoryReceivingVoucherByIdParameter()
            {
                Id = Id
            };
        }
    }
}
