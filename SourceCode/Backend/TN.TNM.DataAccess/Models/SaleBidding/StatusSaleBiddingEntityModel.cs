using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.SaleBidding
{
    public class StatusSaleBiddingEntityModel
    {
        public Guid SaleBiddingId { get; set; }
        public Guid StatusId { get; set; }
        public string Note { get; set; }
    }
}
