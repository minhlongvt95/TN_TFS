using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Contract
{
    public class GetMasterDataDashBoardParameter : BaseParameter
    {
        public int NumberMonth { get; set; }
        public string ContractCode { get; set; }
    }
}
