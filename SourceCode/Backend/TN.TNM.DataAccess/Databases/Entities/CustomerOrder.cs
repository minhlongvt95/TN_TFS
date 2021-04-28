using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CustomerOrder
    {
        public CustomerOrder()
        {
            CustomerOrderDetail = new HashSet<CustomerOrderDetail>();
            OrderCostDetail = new HashSet<OrderCostDetail>();
        }

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
        public Guid? StatusId { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? QuoteId { get; set; }
        public decimal? ReceiptInvoiceAmount { get; set; }
        public string ReasonCancel { get; set; }
        public Guid? TenantId { get; set; }
        public bool? IsAutoGenReceiveInfor { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public Guid? OrderContractId { get; set; }
        public Guid? WarehouseId { get; set; }
        public int? CloneCount { get; set; }

        public Customer Customer { get; set; }
        public Contact CustomerContact { get; set; }
        public OrderStatus Status { get; set; }
        public ICollection<CustomerOrderDetail> CustomerOrderDetail { get; set; }
        public ICollection<OrderCostDetail> OrderCostDetail { get; set; }
    }
}
