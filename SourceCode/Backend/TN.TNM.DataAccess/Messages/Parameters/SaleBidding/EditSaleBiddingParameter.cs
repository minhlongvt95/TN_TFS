using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Lead;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.DataAccess.Messages.Parameters.SaleBidding
{
    public class EditSaleBiddingParameter:BaseParameter
    {
        public SaleBiddingEntityModel SaleBidding { get; set; }
        public List<Guid> ListEmployee { get; set; }
        public List<CostQuoteModel> ListCost { get; set; }
        public List<CostQuoteModel> ListQuocte { get; set; }
        public List<SaleBiddingDetailEntityModel> ListSaleBiddingDetail { get; set; }
    }
}
