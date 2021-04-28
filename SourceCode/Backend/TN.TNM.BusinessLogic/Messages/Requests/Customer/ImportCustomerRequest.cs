using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class ImportCustomerRequest : BaseRequest<ImportCustomerParameter>
    {
        public List<IFormFile> FileList { get; set; }
        public int CustomerType { get; set; }

        public override ImportCustomerParameter ToParameter()
        {
            return new ImportCustomerParameter
            {
                FileList = this.FileList,
                CustomerType = this.CustomerType,
                UserId = this.UserId
            };
        }
    }
}
