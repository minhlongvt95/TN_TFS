using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class SearchContactCustomerResponse : BaseResponse
    {
        public List<ContactEntityModel> ListContact { get; set; }
    }
}
