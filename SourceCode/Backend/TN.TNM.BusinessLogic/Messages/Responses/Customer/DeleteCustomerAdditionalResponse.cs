using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Factories.Customer;
using TN.TNM.BusinessLogic.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class DeleteCustomerAdditionalResponse : BaseResponse
    {
        public List<CustomerAdditionalInformationModel> ListCustomerAdditionalInformation { get; set; }
    }
}
