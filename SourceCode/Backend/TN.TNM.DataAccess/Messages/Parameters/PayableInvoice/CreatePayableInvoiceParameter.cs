namespace TN.TNM.DataAccess.Messages.Parameters.PayableInvoice
{
    public class CreatePayableInvoiceParameter : BaseParameter
    {
        public Databases.Entities.PayableInvoice PayableInvoice { get; set; }
        public Databases.Entities.PayableInvoiceMapping PayableInvoiceMapping { get; set; }
    }
}
