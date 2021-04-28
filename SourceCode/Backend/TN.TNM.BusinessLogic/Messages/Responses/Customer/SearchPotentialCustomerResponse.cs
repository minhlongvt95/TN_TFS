using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class SearchPotentialCustomerResponse : BaseResponse
    {
        public List<DataAccess.Models.Customer.CustomerEntityModel> ListPotentialCustomer { get; set; }
    }
}
