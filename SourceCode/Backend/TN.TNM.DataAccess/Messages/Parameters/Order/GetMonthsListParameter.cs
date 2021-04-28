using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetMonthsListParameter : BaseParameter
    {
        public DateTime? OrderDateFrom { get; set; }
        public DateTime? OrderDateTo { get; set; }
        public int MonthAdd { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
