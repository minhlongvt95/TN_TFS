using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Product
{
    public class DownloadPriceProductTemplateRequest : BaseRequest<DownloadPriceProductTemplateParameter>
    {
        public override DownloadPriceProductTemplateParameter ToParameter()
        {
            return new DownloadPriceProductTemplateParameter
            {
                UserId = this.UserId
            };
        }
    }
}
