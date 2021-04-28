using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;


namespace TN.TNM.DataAccess.Messages.Results.PayableInvoice
{
    public class GetMasterDateSearchBankPayableInvoiceResult : BaseResult
    {
        public List<Category> ReasonOfPaymentList { get; set; }
        public List<Category> StatusOfPaymentList { get; set; }  
        public List<Databases.Entities.Employee> Employees { get; set; }      
    }
}
