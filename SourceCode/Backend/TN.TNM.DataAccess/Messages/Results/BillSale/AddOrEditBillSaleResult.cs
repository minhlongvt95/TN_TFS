using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.BillSale
{
    public class AddOrEditBillSaleResult:BaseResult
    {
        public Guid? BillSaleId { get; set; }
    }
}
