using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class DownloadTemplateProductRequest: BaseRequest<DownloadTemplateProductParameter>
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
