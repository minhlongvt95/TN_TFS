using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class SearchInStockReportResponse : BaseResponse
    {
        public List<InStockEntityModel> ListResult { get; set; }
    }
}
