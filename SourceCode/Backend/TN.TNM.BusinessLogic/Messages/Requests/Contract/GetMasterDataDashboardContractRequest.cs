using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Contract;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contract
{
    public class GetMasterDataDashboardContractRequest : BaseRequest<GetMasterDataDashBoardParameter>
    {
        public int NumberMonth { get; set; }
        public string ContractCode { get; set; }
        public override GetMasterDataDashBoardParameter ToParameter()
        {
            return new GetMasterDataDashBoardParameter
            {
                ContractCode = ContractCode,
                NumberMonth = NumberMonth,
                UserId = UserId
            };
        }
    }
}
