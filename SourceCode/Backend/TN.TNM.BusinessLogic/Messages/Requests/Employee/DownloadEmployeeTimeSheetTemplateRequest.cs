using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class DownloadEmployeeTimeSheetTemplateRequest : BaseRequest<DownloadEmployeeTimeSheetTemplateParameter>
    {
        public override DownloadEmployeeTimeSheetTemplateParameter ToParameter()
        {
            return new DownloadEmployeeTimeSheetTemplateParameter()
            {
                UserId = UserId,
            };
        }
    }
}
