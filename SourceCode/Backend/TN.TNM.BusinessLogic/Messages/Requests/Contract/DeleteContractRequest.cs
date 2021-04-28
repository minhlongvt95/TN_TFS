using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Contract;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contract
{
    public class DeleteContractRequest : BaseRequest<DeleteContractParamter>
    {
        public Guid ContractId { get; set; }
        public override DeleteContractParamter ToParameter()
        {
            return new DeleteContractParamter
            {
                ContractId = ContractId
            };
        }
    }
}
