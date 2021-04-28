using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Contract;

namespace TN.TNM.BusinessLogic.Messages.Responses.Contract
{
    public class SearchContractResponse : BaseResponse
    {
        public List<ContractModel> ListContract { get; set; }
    }
}
