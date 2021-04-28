using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class DownloadTemplatePotentialCustomerRequest : BaseRequest<DownloadTemplatePotentialCustomerParameter>
    {
        public override DownloadTemplatePotentialCustomerParameter ToParameter()
        {
            return new DownloadTemplatePotentialCustomerParameter
            {
                UserId = UserId
            };
        }
    }
}

