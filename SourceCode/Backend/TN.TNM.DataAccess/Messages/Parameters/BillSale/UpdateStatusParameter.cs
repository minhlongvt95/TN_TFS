using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.BillSale
{
    public class UpdateStatusParameter : BaseParameter
    {
        public Guid BillSaleId { get; set; }
        public Guid StatusId { get; set; }
        public string Note { get; set; }
    }
}