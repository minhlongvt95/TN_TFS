using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class UpdateCustomerByIdResult : BaseResult
    {
        public DataAccess.Models.Email.SendEmailEntityModel SendEmailEntityModel { get; set; }
    }
}
