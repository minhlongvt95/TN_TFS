using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.ReceiptInvoice
{
    public class GetMasterDateSearchBankReceiptInvoiceResult : BaseResult
    {
        public List<Category> ReasonOfReceiptList { get; set; }
        public List<Category> StatusOfReceiptList { get; set; }
        public List<Databases.Entities.Employee> Employees { get; set; }
    }
}
