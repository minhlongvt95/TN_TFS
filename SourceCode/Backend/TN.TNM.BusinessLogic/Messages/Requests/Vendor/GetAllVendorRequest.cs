using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class GetAllVendorRequest : BaseRequest<GetAllVendorParameter>
    {
        public override GetAllVendorParameter ToParameter()
        {
            return new GetAllVendorParameter()
            {
                UserId = UserId
            };
        }
    }
}
