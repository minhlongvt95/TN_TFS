using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class GetDataQuoteToPieChartResponse : BaseResponse
    {
        public List<string> CategoriesPieChart { get; set; }
        public List<decimal> DataPieChart { get; set; }
    }
}
