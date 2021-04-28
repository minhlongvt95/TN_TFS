using System;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class CreateCustomerOrderResult : BaseResult
    {
        public Guid CustomerOrderID { get; set; }
        public DataAccess.Models.Email.SendEmailEntityModel SendEmailEntityModel { get; set; }
    }
}
