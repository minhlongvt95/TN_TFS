using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Workflow;

namespace TN.TNM.BusinessLogic.Messages.Requests.Workflow
{
    public class GetMasterDataCreateWorkflowRequest : BaseRequest<GetMasterDataCreateWorkflowParameter>
    {
        public override GetMasterDataCreateWorkflowParameter ToParameter()
        {
            return new GetMasterDataCreateWorkflowParameter()
            {
                UserId = UserId
            };
        }
    }
}
