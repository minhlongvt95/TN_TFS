using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class UpdateCostRequest : BaseRequest<UpdateCostParameter>
    {
        public Guid CostId { get; set; }
        public string CostCode { get; set; }
        public string CostName { get; set; }
        public Guid? OrganzationId { get; set; }
        public Guid? StatusId { get; set; }

        public override UpdateCostParameter ToParameter()
        {
            return new UpdateCostParameter
            {
                CostId = CostId,
                CostCode = CostCode,
                CostName = CostName,
                OrganzationId = OrganzationId,
                StatusId = StatusId
            };
        }
    }
}
