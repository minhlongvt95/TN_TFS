using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetHistoryCustomerCareResult : BaseResult
    {
        public List<CustomerCareInforModel> ListCustomerCareInfor { get; set; }
    }
}
