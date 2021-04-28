using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Contract;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contract
{
    public class GetMasterDataContractRequest : BaseRequest<GetMasterDataContractParameter>
    {
        public Guid? ContractId { get; set; }
        public Guid? QuoteId { get; set; }
        public override GetMasterDataContractParameter ToParameter()
        {
            return new GetMasterDataContractParameter
            {
                ContractId = ContractId,
                UserId = UserId,
                QuoteId = QuoteId
            };
        }
    }
}
