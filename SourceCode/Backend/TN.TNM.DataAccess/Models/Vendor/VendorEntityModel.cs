using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Vendor
{
    public class VendorEntityModel
    {
        public Guid VendorId { get; set; }
        public Guid? ContactId { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public Guid VendorGroupId { get; set; }
        public Guid PaymentId { get; set; }
        public string PaymentName { get; set; }
        public decimal? TotalPurchaseValue { get; set; }
        public decimal? TotalPayableValue { get; set; }
        public DateTime? NearestDateTransaction { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public string VendorGroupName { get; set; }
        public int CountVendorInformation { get; set; }
        public bool CanDelete { get; set; }
        public List<Guid> ListProductId { get; set; }
        public string VendorCodeName { get; set; }

        public VendorEntityModel()
        {
            this.ListProductId = new List<Guid>();
        }
    }
}
