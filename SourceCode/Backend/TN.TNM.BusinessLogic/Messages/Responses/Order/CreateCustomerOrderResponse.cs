using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class CreateCustomerOrderResponse : BaseResponse
    {
        public Guid CustomerOrderID { get; set; }
        public DataAccess.Models.Email.SendEmailEntityModel SendEmailEntityModel { get; set; }
    }
}
