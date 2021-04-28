using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.OrderStatus;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.OrderStatus
{
    public class GetOrderStatusByIDRequest : BaseRequest<GetOrderStatusByIDParameter>
    {
        public Guid OderStatusId { get; set; }

        public override GetOrderStatusByIDParameter ToParameter()
        {
            return new GetOrderStatusByIDParameter() {
                OderStatusId=this.OderStatusId
            };
        }
    }
}
