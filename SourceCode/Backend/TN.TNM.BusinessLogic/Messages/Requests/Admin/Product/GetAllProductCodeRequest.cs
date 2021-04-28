using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Product
{
    public class GetAllProductCodeRequest : BaseRequest<GetAllProductCodeParameter>
    {
        public override GetAllProductCodeParameter ToParameter() => new GetAllProductCodeParameter() { };
    }
}
