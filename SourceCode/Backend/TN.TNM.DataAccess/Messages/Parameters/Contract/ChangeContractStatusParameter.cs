using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Contract
{
    public class ChangeContractStatusParameter : BaseParameter
    {
        public Guid ContractId { get; set; }
        public Guid StatusId { get; set; }
    }
}
