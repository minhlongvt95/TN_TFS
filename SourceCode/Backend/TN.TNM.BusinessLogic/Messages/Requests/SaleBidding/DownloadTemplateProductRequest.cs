using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Requests.SaleBidding
{
    public class DownloadTemplateProductRequest : BaseRequest<DownloadTemplateProductParameter>
    {
        public override DownloadTemplateProductParameter ToParameter()
        {
            return new DownloadTemplateProductParameter()
            {
                UserId = UserId
            };
        }
    }
}
