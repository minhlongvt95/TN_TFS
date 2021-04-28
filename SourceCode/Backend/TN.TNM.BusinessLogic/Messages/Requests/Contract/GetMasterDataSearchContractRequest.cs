using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Contract;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contract
{
    public class GetMasterDataSearchContractRequest : BaseRequest<GetMasterDataSearchContractParameter>
    {
        public override GetMasterDataSearchContractParameter ToParameter()
        {
            return new GetMasterDataSearchContractParameter
            {
                UserId = UserId,
            };
        }
    }
}
