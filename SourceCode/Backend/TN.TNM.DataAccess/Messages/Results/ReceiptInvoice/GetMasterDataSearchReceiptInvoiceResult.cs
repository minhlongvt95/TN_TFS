using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.ReceiptInvoice
{
    public class GetMasterDataSearchReceiptInvoiceResult : BaseResult
    {
        public List<CategoryEntityModel> ReasonOfReceiptList { get; set; }
        public List<CategoryEntityModel> StatusOfReceiptList { get; set; }
        public List<Databases.Entities.Employee> Employees { get; set; }
        
    }
}
