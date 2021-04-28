using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.BillSale;

namespace TN.TNM.DataAccess.Messages.Parameters.BillSale
{
    public class AddOrEditBillSaleParameter:BaseParameter
    {
        public bool? IsCreate { get; set; }
        public BillSaleEntityModel BillSale { get; set; }
    }
}
