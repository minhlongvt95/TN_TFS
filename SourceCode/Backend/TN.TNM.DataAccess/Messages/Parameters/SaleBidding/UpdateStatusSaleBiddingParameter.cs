using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.DataAccess.Messages.Parameters.SaleBidding
{
    public class UpdateStatusSaleBiddingParameter:BaseParameter
    {
        public List<StatusSaleBiddingEntityModel> ListStaus { get; set; }
    }
}
