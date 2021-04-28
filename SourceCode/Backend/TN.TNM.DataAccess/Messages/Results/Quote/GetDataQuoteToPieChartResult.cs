using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class GetDataQuoteToPieChartResult : BaseResult
    {
        public List<string> CategoriesPieChart { get; set; }
        public List<decimal> DataPieChart { get; set; }
    }
}
