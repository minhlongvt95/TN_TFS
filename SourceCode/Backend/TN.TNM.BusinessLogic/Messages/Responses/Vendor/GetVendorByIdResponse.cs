using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class GetVendorByIdResponse : BaseResponse
    {
        public VendorModel Vendor { get; set; }
        public ContactModel Contact { get; set; }
        public List<BankAccountModel> VendorBankAccountList { get; set; }
        public List<ContactModel> VendorContactList { get; set; }
        public string FullAddress { get; set; }
        public int CountVendorInformation { get; set; }
        public List<DataAccess.Models.Vendor.ExchangeByVendorEntityModel> ListExchangeByVendor { get; set; }
    }
}
