using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class CheckBeforCreateOrUpdateOrderRequest : BaseRequest<CheckBeforCreateOrUpdateOrderParameter>
    {
        public Guid CustomerId { get; set; }
        public decimal MaxDebt { get; set; }
        public decimal AmountOrder { get; set; }
        public override CheckBeforCreateOrUpdateOrderParameter ToParameter()
        {
            return new CheckBeforCreateOrUpdateOrderParameter
            {
                CustomerId = CustomerId,
                MaxDebt = MaxDebt,
                AmountOrder = AmountOrder,
                UserId =this.UserId
            };
        }
    }
}
