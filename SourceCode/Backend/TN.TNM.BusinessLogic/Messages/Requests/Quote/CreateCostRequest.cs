using System;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class CreateCostRequest : BaseRequest<CreateCostParameter>
    {
        public string CostCode { get; set; }
        public string CostName { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? OrganzationId { get; set; }
        public override CreateCostParameter ToParameter()
        {
            return new CreateCostParameter
            {
                CostCode = CostCode,
                CostName = CostName,
                StatusId = StatusId,
                OrganzationId = OrganzationId,
                UserId = UserId
            };
        }
    }
}
