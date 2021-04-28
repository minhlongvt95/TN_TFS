using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Contract;

namespace TN.TNM.DataAccess.Messages.Results.Contract
{
    public class GetMasterDataDashBoardResult : BaseResult
    {
        public List<ContractDashboardEntityModel> ListValueOfStatus { get; set; }
        public List<ContractDashboardEntityModel> ListValueOfMonth { get; set; }
        public List<ContractEntityModel> ListContractWorking { get; set; }
        public List<ContractEntityModel> ListContractNew { get; set; }
        public List<ContractEntityModel> ListContractExpiredDate { get; set; }

    }
}
