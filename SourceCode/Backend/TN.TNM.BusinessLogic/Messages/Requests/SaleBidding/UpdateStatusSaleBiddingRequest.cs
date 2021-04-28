using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Requests.SaleBidding
{
    public class UpdateStatusSaleBiddingRequest : BaseRequest<UpdateStatusSaleBiddingParameter>
    {
        public List<StatusSaleBiddingEntityModel> ListStaus { get; set; }
        
        public override UpdateStatusSaleBiddingParameter ToParameter()
        {
            return new UpdateStatusSaleBiddingParameter()
            {
                ListStaus = ListStaus,
                UserId = UserId,
            };
        }
    }
}
