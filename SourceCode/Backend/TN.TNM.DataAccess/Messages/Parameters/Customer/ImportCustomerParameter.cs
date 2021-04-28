using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class ImportCustomerParameter:BaseParameter
    {
        public List<IFormFile> FileList { get; set; }
        public int CustomerType { get; set; }
    }
}
