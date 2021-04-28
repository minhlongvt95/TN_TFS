using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.BusinessLogic.Models.ReceiptInvoice
{
    public class CustomerOrderModel : BaseModel<CustomerOrder>
    {
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid? Seller { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? CustomerContactId { get; set; }
        public Guid? PaymentMethod { get; set; }
        public bool? DiscountType { get; set; }
        public Guid? BankAccountId { get; set; }
        public int? DaysAreOwed { get; set; }
        public decimal? MaxDebt { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public TimeSpan? ReceivedHour { get; set; }
        public string RecipientName { get; set; }
        public string LocationOfShipment { get; set; }
        public string ShippingNote { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientEmail { get; set; }
        public string PlaceOfDelivery { get; set; }
        public decimal Amount { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? ReceiptInvoiceAmount { get; set; }
        public Guid? StatusId { get; set; }
        public string StatusCode { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public Guid SellerContactId { get; set; }
        public string SellerFirstName { get; set; }
        public string SellerLastName { get; set; }
        public string SellerAvatarUrl { get; set; }
        public string OrderStatusName { get; set; }
        public string CustomerName { get; set; }
        public int TypeAccount { get; set; }
        public string ListOrderDetail { get; set; }
        public string ReasonCancel { get; set; }
        public Guid? QuoteId { get; set; }

        public CustomerOrderModel() { }
        public CustomerOrderModel(CustomerOrder entity) : base(entity)
        {
            //            Mapper(entity, this);
        }
        public CustomerOrderModel(CustomerOrderEntityModel model)
        {
            Mapper(model, this);
        }
        public override CustomerOrder ToEntity()
        {
            var entity = new CustomerOrder();
            Mapper(this, entity);
            return entity;
        }
    }
}
