using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class DeleteListCustomerAdditionalResult : BaseResult
    {
        public List<CustomerAdditionalInformation> ListCustomerAdditionalInformation { get; set; }
    }
}
