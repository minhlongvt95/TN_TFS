using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class DownloadTemplateImportCustomerRequest : BaseRequest<DownloadTemplateImportCustomerParameter>
    {
        public override DownloadTemplateImportCustomerParameter ToParameter()
        {
            return new DownloadTemplateImportCustomerParameter
            {
                UserId = UserId
            };
        }
    }
}
