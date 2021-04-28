using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class UpdateStatusOrderRequest : BaseRequest<UpdateStatusOrderParameter>
    {
        public Guid CustomerOrderId { get; set; }
        public string ObjectType { get; set; }
        public string Description { get; set; }
        public override UpdateStatusOrderParameter ToParameter()
        {
            return new UpdateStatusOrderParameter
            {
                CustomerOrderId = CustomerOrderId,
                ObjectType = ObjectType,
                Description = Description,
                UserId =this.UserId
            };
        }
    }
}
