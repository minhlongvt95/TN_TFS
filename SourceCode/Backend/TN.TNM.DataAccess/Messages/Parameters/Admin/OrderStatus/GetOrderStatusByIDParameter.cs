using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.OrderStatus
{
    public class GetOrderStatusByIDParameter : BaseParameter
    {
        public Guid OderStatusId { get; set; }
    }
}
