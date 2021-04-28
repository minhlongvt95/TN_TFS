using System;
using TN.TNM.DataAccess.Models.Receivable;

namespace TN.TNM.BusinessLogic.Models.Receivable
{
    public class ReceivableCustomerModel : BaseModel<ReceivableCustomerEntityModel>
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public DateTime? NearestTransaction { get; set; }
        public decimal? TotalSales { get; set; }
        public decimal? TotalPaid { get; set; }
        public decimal? TotalUnpaid { get; set; }
        public decimal? TotalReceipt { get; set; }  //Tổng còn phải thu
        public decimal? ReceiptInvoiceValue { get; set; }
        public decimal? OrderValue { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ReceiptId { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? ProductId { get; set; }
        public Guid CustomerOrderId { get; set; }
        public DateTime? CreateDateOrder { get; set; }
        public DateTime? CreateDateReceiptInvoice { get; set; }
        public string OrderCode { get; set; }
        public string OrderName { get; set; }
        public string CreatedByName { get; set; }
        public string DescriptionReceipt { get; set; }
        public string ReceiptCode { get; set; }
        public Guid? Status { get; set; }
        public string Router { get; set; }

        public ReceivableCustomerModel() { }
        public ReceivableCustomerModel(ReceivableCustomerEntityModel model)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(model, this);
        }
        public ReceivableCustomerEntityModel ToModel()
        {
            //Code tien xu ly model truoc khi day vao DB
            var model = new ReceivableCustomerEntityModel();
            Mapper(this, model);
            return model;
        }

        public override ReceivableCustomerEntityModel ToEntity()
        {
            throw new NotImplementedException();
        }
    }
}
