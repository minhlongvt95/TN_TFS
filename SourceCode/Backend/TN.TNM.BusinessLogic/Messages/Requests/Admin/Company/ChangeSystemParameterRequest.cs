using TN.TNM.DataAccess.Messages.Parameters.Admin.Company;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Company
{
    public class ChangeSystemParameterRequest : BaseRequest<ChangeSystemParameterParameter>
    {
        public string SystemKey { get; set; }
        public bool? SystemValue { get; set; }
        public string SystemValueString { get; set; }
        public string Description { get; set; }
        public override ChangeSystemParameterParameter ToParameter() => new ChangeSystemParameterParameter
        {
            SystemKey = SystemKey,
            SystemValue = SystemValue,
            SystemValueString = SystemValueString,
            Description = Description,
            UserId = UserId
        };
    }
}
