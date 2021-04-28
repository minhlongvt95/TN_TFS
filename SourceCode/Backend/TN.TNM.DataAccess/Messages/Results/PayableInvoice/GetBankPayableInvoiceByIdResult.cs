using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.PayableInvoice;

namespace TN.TNM.DataAccess.Messages.Results.PayableInvoice
{
    public class GetBankPayableInvoiceByIdResult : BaseResult
    {
        public BankPayableInvoiceEntityModel BankPayableInvoice { get; set; }
        //public string CreatedName { get; set; }
        //public string ObjectName { get; set; }
    }
}
