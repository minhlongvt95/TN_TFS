using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class ExportAssistantRequest : BaseRequest<ExportAssistantParameter>
    {
        public int Month { get; set; }
        public int Year { get; set; }

        public override ExportAssistantParameter ToParameter()
        {
            return new ExportAssistantParameter()
            {
                Month=this.Month,
                Year=this.Year,
                UserId=this.UserId
            };
        }
    }
}
