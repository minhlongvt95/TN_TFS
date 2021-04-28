namespace TN.TNM.BusinessLogic.Models.Order
{
    public class PDFOrderAttributeModel
    {
        public string Stt { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }    //Đơn vị tính
        public string Quantity { get; set; }
        public string UnitPrice { get; set; }
        public string ExchangeRate { get; set; }
        public string VAT { get; set; }
        public string DiscountValue { get; set; }
        public string Amount { get; set; }
    }
}
