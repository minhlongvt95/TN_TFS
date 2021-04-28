using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class CheckExistsCustomerByPhoneResult : BaseResult
    {
        public CustomerEntityModel Customer { get; set; }
        public decimal PointRate { get; set; }
    }
}
