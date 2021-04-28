using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class CheckExistsCustomerByPhoneResponse : BaseResponse
    {
        public CustomerEntityModel Customer { get; set; }
        public decimal PointRate { get; set; }
    }
}
