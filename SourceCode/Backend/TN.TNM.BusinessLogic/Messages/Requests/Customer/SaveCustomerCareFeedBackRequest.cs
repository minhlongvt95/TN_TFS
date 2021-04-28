using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.CustomerCare;
using TN.TNM.DataAccess.Messages.Parameters.Customer;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class SaveCustomerCareFeedBackRequest : BaseRequest<SaveCustomerCareFeedBackParameter>
    {
        public CustomerCareFeedBackModel CustomerCareFeedBack { get; set; }
        public override SaveCustomerCareFeedBackParameter ToParameter()
        {
            return new SaveCustomerCareFeedBackParameter()
            {
                UserId = UserId,
                CustomerCareFeedBack = new SaveCustomerCareFeedBackModel(CustomerCareFeedBack.CustomerId.Value, CustomerCareFeedBack.CustomerCareId.Value, CustomerCareFeedBack.FeedBackCode.Value, CustomerCareFeedBack.FeedBackContent),
            };
        }
    }
}
