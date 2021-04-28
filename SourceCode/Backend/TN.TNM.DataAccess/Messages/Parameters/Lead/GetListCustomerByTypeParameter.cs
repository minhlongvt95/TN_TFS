using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class GetListCustomerByTypeParameter : BaseParameter
    {
        public string CustomerType { get; set; }
    }
}
