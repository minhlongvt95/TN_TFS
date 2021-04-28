using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetDataLeadProductDialogRequest : BaseRequest<GetDataLeadProductDialogParameter>
    {
        public override GetDataLeadProductDialogParameter ToParameter()
        {
            return new GetDataLeadProductDialogParameter()
            {
                UserId = UserId
            };
        }
    }
}
