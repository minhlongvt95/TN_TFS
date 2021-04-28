using TN.TNM.DataAccess.Messages.Parameters.Admin.OrderStatus;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.OrderStatus
{
    public class GetAllOrderStatusRequest : BaseRequest<GetAllOrderStatusParameter>
    {

        public override GetAllOrderStatusParameter ToParameter()
        {
            return new GetAllOrderStatusParameter() { };
        }
    }
}
