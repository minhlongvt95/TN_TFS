using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Lead;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.DataAccess.Messages.Parameters.SaleBidding
{
    public class CreateSaleBiddingParameter:BaseParameter
    {
        public SaleBiddingEntityModel SaleBidding { get; set; }
        public List<Guid> ListEmployee { get; set; }
        public List<LeadDetailModel> ListCost { get; set; }
        public List<LeadDetailModel> ListQuocte { get; set; }
        public List<SaleBiddingDetailEntityModel> ListSaleBiddingDetail { get; set; }
    }
}
