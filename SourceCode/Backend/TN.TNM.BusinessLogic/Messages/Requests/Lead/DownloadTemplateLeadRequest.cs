using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class DownloadTemplateLeadRequest : BaseRequest<DownloadTemplateLeadParameter>
    {

        public override DownloadTemplateLeadParameter ToParameter()
        {
            return new DownloadTemplateLeadParameter
            {
                UserId=this.UserId
            };
        }
    }
}
