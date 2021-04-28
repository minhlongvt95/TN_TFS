using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Contract
{
    public class CreateCloneContractResult : BaseResult
    {
        public Guid ContractId { get; set; }
    }
}
