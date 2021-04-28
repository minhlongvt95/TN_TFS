using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.BillSale;

namespace TN.TNM.BusinessLogic.Messages.Responses.BillSale
{
    public class SearchBillOfSaleResponse : BaseResponse
    {
        public List<BillSaleModel> ListBillOfSale { get; set; }
    }
}
