using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Vendor
{
    public class VendorCreateOrderEntityModel
    {
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public Guid? PaymentId { get; set; }
        public string VendorEmail { get; set; }
        public string VendorPhone { get; set; }
        public string FullAddressVendor { get; set; }
        public string VendorCode { get; set; }
        public List<Models.ContactEntityModel> ListVendorContact { get; set; }
        
        public VendorCreateOrderEntityModel()
        {
            this.ListVendorContact = new List<ContactEntityModel>();
        }
    }
}
