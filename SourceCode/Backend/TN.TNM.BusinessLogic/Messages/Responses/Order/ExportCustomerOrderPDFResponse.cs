using TN.TNM.BusinessLogic.Models.Order;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class ExportCustomerOrderPDFResponse : BaseResponse
    {
        public string Code { get; set; }
        public PDFOrderModel PDFOrder { get; set; }
    }
}
