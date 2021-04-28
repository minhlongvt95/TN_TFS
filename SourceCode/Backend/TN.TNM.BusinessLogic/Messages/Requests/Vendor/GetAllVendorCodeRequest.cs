using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class GetAllVendorCodeRequest : BaseRequest<GetAllVendorCodeParameter>
    {
        public override GetAllVendorCodeParameter ToParameter()
        {
            return new GetAllVendorCodeParameter()
            {
                UserId = UserId
            };
        }
    }
}
