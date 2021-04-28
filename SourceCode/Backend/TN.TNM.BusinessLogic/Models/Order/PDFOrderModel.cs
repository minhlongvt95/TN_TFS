using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Models.Order
{
    public class PDFOrderModel
    {
        public string OrderDate { get; set; }
        public string CompanyName { get; set; } //Tên Cty
        public string TaxCode { get; set; } //Mã số thuế của Cty
        public string CompanyPhone { get; set; }    //Sdt Cty
        public string CompanyEmail { get; set; }    //Email Cty
        public string Website { get; set; }    //website Cty
        public string CompanyAddress { get; set; }
        public string OrderCode { get; set; }
        public string Seller { get; set; }  //nv bán hàng
        public string LocationOfShipment { get; set; }  //Địa chỉ xuất hàng
        public string Description { get; set; } //Diễn giải
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerTaxCode { get; set; }
        public string CustomerPaymentMethod { get; set; }
        public string RecipientName { get; set; }   //Người nhận hàng
        public string PlaceOfDelivery { get; set; } //Địa chỉ nhận
        public string ReceivedDate { get; set; } //Ngày/giờ nhận
        public string ShippingNote { get; set; } //Ghi chú
        public string TotalAmount { get; set; } //Tổng tiền hóa đơn
        public string DiscountValue { get; set; } //Chiết khấu cho cả đơn hàng (VND)
        public string TotalVat { get; set; } //Tổng tiền thuế (VND)
        public string TotalBeforVat { get; set; } //Tổng tiền trước thuế (VND)
        public string TotalDiscountValue { get; set; } //Tổng tiền chiết khấu (VND)
        public string TotalAmountAfter { get; set; } //Tổng tiền hóa đơn sau khi tính chiết khấu
        public string TotalAmountAfterText { get; set; } //Tổng tiền hóa đơn sau khi tính chiết khấu bằng chữ 
        public List<PDFOrderAttributeModel> ListPDFOrderAttribute { get; set; }
        public List<PDFOrderAttributeModel> ListPDFOrderAttributeOther { get; set; } //Danh sách các item khác phát sinh trong đơn hàng

        public PDFOrderModel() { }
    }
}
