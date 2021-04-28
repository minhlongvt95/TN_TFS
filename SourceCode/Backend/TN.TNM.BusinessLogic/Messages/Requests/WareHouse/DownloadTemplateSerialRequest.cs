using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class DownloadTemplateSerialRequest : BaseRequest<DownloadTemplateSerialParameter>
    {
        public override DownloadTemplateSerialParameter ToParameter()
        {
            return new DownloadTemplateSerialParameter()
            {
                UserId = UserId,
            };
        }
    }
}
