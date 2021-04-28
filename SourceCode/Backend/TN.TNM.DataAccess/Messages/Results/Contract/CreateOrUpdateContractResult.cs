using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Contract
{
    public class CreateOrUpdateContractResult : BaseResult
    {
        public Guid ContractId { get; set; }
    }
}
