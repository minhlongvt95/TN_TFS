using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Contract;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contract
{
    public class ChangeContractStatusRequest : BaseRequest<ChangeContractStatusParameter>
    {
        public Guid ContractId { get; set; }
        public Guid StatusId { get; set; }
        public override ChangeContractStatusParameter ToParameter()
        {
            return new ChangeContractStatusParameter
            {
                ContractId = ContractId,
                StatusId = StatusId,
            };
        }
    }
}
