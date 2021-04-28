using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Contract
{
    public class DeleteContractParamter : BaseParameter
    {
       public Guid ContractId { get; set; }
    }
}
