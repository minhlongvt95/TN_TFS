using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class GetListCustomerParameter:BaseParameter
    {
        public string CategoryTypeCode { get; set; }
    }
}
