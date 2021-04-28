using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Contract
{
    public class CreateCloneContractResponse : BaseResponse
    {
        public Guid ContractId { get; set; }
    }
}
