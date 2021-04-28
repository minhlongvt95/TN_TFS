using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.CashBook
{
    public class GetSurplusCashBookPerMonthParameter : BaseParameter
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<Guid?> OrganizationList { get; set; }
    }
}
