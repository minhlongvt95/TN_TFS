using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetCustomerImportDetailResult:BaseResult
    {
        public List<Databases.Entities.Category> ListCustomerGroup { get; set; }
        public List<string> ListCustomerCompanyCode { get; set; }
        public List<string> ListEmail { get; set; }
        public List<string> ListPhone { get; set; }
    } 
}
