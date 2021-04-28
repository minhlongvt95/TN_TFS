using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Contract;
using TN.TNM.DataAccess.Models.Contract;

namespace TN.TNM.BusinessLogic.Messages.Responses.Contract
{
    public class GetMasterDataDashboardContractResponse : BaseResponse
    {
        public List<ContractDashboardEntityModel> ListValueOfStatus { get; set; }
        public List<ContractDashboardEntityModel> ListValueOfMonth { get; set; }
        public List<ContractModel> ListContractWorking { get; set; }
        public List<ContractModel> ListContractNew { get; set; }
        public List<ContractModel> ListContractExpiredDate { get; set; }

    }
}
