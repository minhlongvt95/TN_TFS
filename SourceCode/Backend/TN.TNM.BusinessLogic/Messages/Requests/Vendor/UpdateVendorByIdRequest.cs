using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class UpdateVendorByIdRequest : BaseRequest<UpdateVendorByIdParameter>
    {
        public VendorModel Vendor { get; set; }
        public ContactModel Contact { get; set; }
        public override UpdateVendorByIdParameter ToParameter()
        {
            return new UpdateVendorByIdParameter() {
                UserId = UserId,
                Vendor = Vendor.ToEntity(),
                Contact = Contact.ToEntity()
            };
        }
    }
}
