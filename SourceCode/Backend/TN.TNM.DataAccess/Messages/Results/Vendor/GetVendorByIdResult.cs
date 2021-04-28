using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetVendorByIdResult : BaseResult
    {
        public VendorEntityModel Vendor { get; set; }
        public ContactEntityModel Contact { get; set; }
        public string FullAddress { get; set; }
        public List<Databases.Entities.BankAccount> VendorBankAccountList { get; set; }
        public List<Databases.Entities.Contact> VendorContactList { get; set; }
        public int CountVendorInformation { get; set; }
        public List<Models.Vendor.ExchangeByVendorEntityModel> ListExchangeByVendor { get; set; }
    }
}
