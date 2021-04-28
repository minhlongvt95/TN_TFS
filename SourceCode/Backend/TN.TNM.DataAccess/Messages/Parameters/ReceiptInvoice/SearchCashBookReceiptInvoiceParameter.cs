using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice
{
    public class SearchCashBookReceiptInvoiceParameter : BaseParameter
    {
        public List<Guid> CreateById { get; set; }
        public DateTime? ReceiptDateFrom { get; set; }
        public DateTime? ReceiptDateTo { get; set; }
        public List<Guid?> OrganizationList { get; set; }
    }
}
