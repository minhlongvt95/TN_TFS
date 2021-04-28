using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Email
{
    public class SendEmailEntityModel
    {
        //gửi email khi tạo lead
        public string LeadName { get; set; }
        public string LeadEmail { get; set; }
        public string LeadPhone { get; set; }
        public string LeadAddress { get; set; }
        public string LeadInterested { get; set; }
        public string LeadPotential { get; set; }
        public string LeadPicCode { get; set; }
        public string LeadPicName { get; set; }    
        public Guid LeadId { get; set; }
        public Guid LeadContactId { get; set; }

        //gửi email khi tạo báo giá
        public string QuoteCode { get; set; }
        public string QuoteStatus { get; set; }
        public string CustomerType { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string Seller { get; set; }
        public string SendQuoteDate { get; set; }
        public string EffectiveQuoteDate { get; set; }
        public string CreatedEmployeeName { get; set; }
        public string CreatedEmployeeCode { get; set; }
        public bool SendDetailProduct { get; set; } //gui bang detail product
        public List<QuoteDetailToSendEmailModel> ListQuoteDetailToSendEmail { get; set; }
        public string SumAmountDiscount { get; set; } //tổng tiền chiết khấu theo sản phẩm
        public string AmountDiscountByQuote { get; set; } //chiết khấu theo quote
        public string SumAmount { get; set; } //tổng tiền theo sản phẩm
        public string SumAmountByQuote { get; set; }//tổng tiền theo qute
        public string SumAmountTransform { get; set; }

        //gửi email khi tạo khách hàng
        //public string CustomerName { get; set; }
        //public string CustomerType { get; set; }
        public string CustomerGroup { get; set; }
        public string CustomerCode { get; set; }
        //public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerSeller { get; set; }
        
        //gui email sau khi tao nhan vine
        //public string EmployeeName { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }

        //gui email sau khi tao don hang
        public string OrderCode { get; set; }
        public string OrderStatus { get; set; }
        //public string CompanyName { get; set; }
        //public string CustomerName 
        public string OrderDate { get; set; }
        public string RecipientName { get; set; }
        public string PlaceOfDelivery { get; set; }
        public string ReceivedDateHour { get; set; }
        public string RecipientPhone { get; set; }
        public string DetailOrder { get; set; }
        public bool SendDetailProductInOrder { get; set; } //gui bang detail product
        public List<OrderDetailToSendEmailModel> ListDetailToSendEmailInOrder { get; set; }
        public string SumAmountDiscountByProductInOder { get; set; } //tổng tiền chiết khấu theo sản phẩm
        public string SumAmountDiscountByOrder { get; set; } //chiết khấu theo order
        public string SumAmountBeforeDiscount { get; set; } //tổng tiền order truoc chiet khau
        public string SumAmountAfterDiscount { get; set; }//tổng tiền sau chiet khau
        public string SumAmountTransformInOrder { get; set; }

        //gui email sau khi tao de xuat xin nghi
        public List<string> ListEmployeeName_1 { get; set; } //danh sách tên những người nhận email theo template 1: thông báo
        public List<string> ListEmployeeName_2 { get; set; } //danh sách tên những người nhận email theo template 2: phê duyệt
        public string EmployeeRequestCode { get; set; }
        public string OfferEmployeeName { get; set; }
        public string CreateEmployeeCode { get; set; }
        public string CreateEmployeeName { get; set; }
        public string CreatedDate { get; set; }
        public string TypeRequestName { get; set; }
        public string DurationTime { get; set;} //thời gian nghỉ
        public string Detail { get; set; }
        public string ApproverCode { get; set; }
        public string ApproverName { get; set; }
        public string NotifyList { get; set; }

        //danh sach gui email khi tao de xuat xin nghi
        public List<string> ListEmployeeSendEmail_1 { get; set; } // gửi đến email theo template 1
        public List<string> ListEmployeeSendEmail_2 { get; set; } // gửi đến email theo template 1
        public List<string> ListEmployeeCCEmail{ get; set; } //list cc

        //Gửi email sau khi gửi phê duyệt phiếu đề xuất mua hàng
        public string OrganizationName { get; set; }
        public string ProcurementCode { get; set; }

        //genaral
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }

        //list send to email
        public List<string> ListSendToEmail { get; set; }

        public SendEmailEntityModel()
        {
            this.ListSendToEmail = new List<string>();
            this.ListEmployeeName_1 = new List<string>();
            this.ListEmployeeName_2 = new List<string>();
            this.ListEmployeeSendEmail_1 = new List<string>();
            this.ListEmployeeSendEmail_2 = new List<string>();
            this.ListEmployeeCCEmail = new List<string>();
        }
    }

    public class QuoteDetailToSendEmailModel
    {
        public string ProductName { get; set; }
        public string ProductNameUnit { get; set; }
        public string UnitPrice { get; set; }
        public string Quantity { get; set; }
        public string Vat { get; set; }
        public string DiscountValue { get; set; }
        public string SumAmount { get; set; }
        public string AmountDiscountPerProduct { get; set; }
    }

    public class OrderDetailToSendEmailModel
    {
        public string ProductName { get; set; }
        public string ProductNameUnit { get; set; }
        public string UnitPrice { get; set; }
        public string Quantity { get; set; }
        public string Vat { get; set; }
        public string DiscountValue { get; set; }
        public string SumAmount { get; set; }
        public string AmountDiscountPerProduct { get; set; }
    }
}
