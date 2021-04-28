using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class QuickCreateVendorRequest : BaseRequest<QuickCreateVendorParameter>
    {
        public VendorModel Vendor { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public override QuickCreateVendorParameter ToParameter()
        {
            return new QuickCreateVendorParameter() {
                Vendor = Vendor.ToEntity(),
                UserId = UserId,
                Email = Email,
                Address = Address,
                Phone = Phone
            };
        }
    }
}
