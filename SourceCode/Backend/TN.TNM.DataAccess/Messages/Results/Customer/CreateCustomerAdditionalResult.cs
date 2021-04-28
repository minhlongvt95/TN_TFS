using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class CreateCustomerAdditionalResult: BaseResult
    {
        public List<CustomerAdditionalInformation> ListCustomerAdditionalInformation { get; set; }
    }
}
