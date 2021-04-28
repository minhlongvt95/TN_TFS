using System.Collections.Generic;
using TN.TNM.DataAccess.Models.ReceiptInvoice;

namespace TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice
{
    public class CreateReceiptInvoiceParameter : BaseParameter
    {
        public Databases.Entities.ReceiptInvoice ReceiptInvoice { get; set; }
        public Databases.Entities.ReceiptInvoiceMapping ReceiptInvoiceMapping { get; set; }
        public List<ReceiptHistoryEntityModel> ReceiptOrderHistory { get; set; }
    }
}
