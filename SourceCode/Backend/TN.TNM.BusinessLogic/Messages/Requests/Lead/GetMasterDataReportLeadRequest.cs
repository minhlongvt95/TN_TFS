using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetMasterDataReportLeadRequest : BaseRequest<GetMasterDataReportLeadParameter>
    {
        public override GetMasterDataReportLeadParameter ToParameter()
        {
            return new GetMasterDataReportLeadParameter
            {
                UserId = this.UserId
            };
        }
    }
}
