using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Customer
{
    public class SaveCustomerCareFeedBackModel
    {
        public Guid CustomerId { get; set; }
        public Guid CustomerCareId { get; set; }
        public Guid FeedBackCode { get; set; }
        public string FeedBackContent { get; set; }

        public SaveCustomerCareFeedBackModel()
        {

        }

        public SaveCustomerCareFeedBackModel(Guid _customerID, Guid _customerCareId, Guid _feedBackCode, string _feedbackContent)
        {
            CustomerId = _customerID;
            CustomerCareId = _customerCareId;
            FeedBackCode = _feedBackCode;
            FeedBackContent = _feedbackContent;
        }
    }
}
