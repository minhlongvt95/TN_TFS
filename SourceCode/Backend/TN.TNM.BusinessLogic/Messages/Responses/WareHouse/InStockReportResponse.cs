using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class InStockReportResponse:BaseResponse
    {
        public List<InStockModel> lstResult { get; set; }
    }
}
