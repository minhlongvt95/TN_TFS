using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Models.Vendor
{
    public class VendorOrderModel : BaseModel<VendorOrder>
    {
        public Guid VendorOrderId { get; set; }
        public string VendorOrderCode { get; set; }
        public DateTime VendorOrderDate { get; set; }
        public Guid? CustomerOrderId { get; set; }
        public Guid? Orderer { get; set; }
        public string OrdererName { get; set; }
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
        public string CreatedByName { get; set; }
        public string AvatarUrl { get; set; }
        public string StatusName { get; set; }
        public string StatusCode { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public Guid? WarehouseId { get; set; }
        public string ShipperName { get; set; }
        public string TypeCost { get; set; }
        public string ListProcurementName { get; set; }

        public VendorOrderModel() { }

        public VendorOrderModel(VendorOrder entity) : base(entity)
        {
            Mapper(entity, this);
        }

        public VendorOrderModel(VendorOrderEntityModel model)
        {
            Mapper(model, this);
        }

        public override VendorOrder ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new VendorOrder();
            Mapper(this, entity);
            return entity;
        }
    }
}
