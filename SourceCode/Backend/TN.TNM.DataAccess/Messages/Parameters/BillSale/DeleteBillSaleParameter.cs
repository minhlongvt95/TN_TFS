using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.BillSale
{
    public class DeleteBillSaleParameter:BaseParameter
    {
        public Guid BillSaleId { get; set; }
    }
}
