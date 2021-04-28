using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.CustomerCare
{
    public class FilterCustomerParameter:BaseParameter
    {
       public string SqlQuery { get; set; }
       public List<string> CustomerStatusCode { get; set; }
    }
}
