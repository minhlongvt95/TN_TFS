using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetMonthsListResult : BaseResult
    {
        public List<dynamic> monthOrderList { get; set; }
    }
}
