using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class ExportCustomerOrderPDFResult : BaseResult
    {
        public string Code { get; set; }
        public PDFOrderModel PDFOrder { get; set; }
    }
}
