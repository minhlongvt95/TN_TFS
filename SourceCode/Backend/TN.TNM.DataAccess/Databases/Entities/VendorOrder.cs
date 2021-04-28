using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class VendorOrder
    {
        public VendorOrder()
        {
            VendorOrderCostDetail = new HashSet<VendorOrderCostDetail>();
        }

        public Guid VendorOrderId { get; set; }
        public string VendorOrderCode { get; set; }
        public DateTime VendorOrderDate { get; set; }
        public Guid? CustomerOrderId { get; set; }
        public Guid? Orderer { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public Guid VendorId { get; set; }
        public Guid? VendorContactId { get; set; }
        public Guid? PaymentMethod { get; set; }
        public Guid? BankAccountId { get; set; }
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
        public Guid StatusId { get; set; }
        public bool? DiscountType { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? ApproverId { get; set; }
        public Guid? ApproverPostion { get; set; }
        public Guid? ContractId { get; set; }
        public Guid? WarehouseId { get; set; }
        public string ShipperName { get; set; }
        public string TypeCost { get; set; }

        public ICollection<VendorOrderCostDetail> VendorOrderCostDetail { get; set; }
    }
}
