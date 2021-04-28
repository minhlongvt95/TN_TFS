using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.BillSale;

namespace TN.TNM.DataAccess.Messages.Results.BillSale
{
    public class SearchBillOfSaleResult : BaseResult
    {
        public List<BillSaleEntityModel> ListBillOfSale { get; set; }
    }
}
