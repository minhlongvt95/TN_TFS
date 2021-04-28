using TN.TNM.DataAccess.Messages.Parameters.Admin.Province;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Province
{
    public class GetAllProvinceRequest : BaseRequest<GetAllProvinceParameter>
    {
        public override GetAllProvinceParameter ToParameter()
        {
            return new GetAllProvinceParameter() { };
        }
    }
}
