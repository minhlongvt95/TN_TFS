using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.ReceiptInvoice;

namespace TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice
{
    public class CreateBankReceiptInvoiceParameter: BaseParameter
    {
        public BankReceiptInvoice BankReceiptInvoice { get; set; }
        public BankReceiptInvoiceMapping BankReceiptInvoiceMapping { get; set; }
        public List<ReceiptHistoryEntityModel> ReceiptOrderHistory { get; set; }
    }
}
