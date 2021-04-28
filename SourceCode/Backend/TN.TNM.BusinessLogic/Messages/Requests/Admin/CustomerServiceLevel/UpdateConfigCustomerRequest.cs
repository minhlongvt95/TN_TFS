using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.CustomerServiceLevel;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.CustomerServiceLevel
{
    public class UpdateConfigCustomerRequest : BaseRequest<UpdateConfigCustomerParameter>
    {
        public Guid CustomerLevelId { get; set; }
        public override UpdateConfigCustomerParameter ToParameter()
        {
            return new UpdateConfigCustomerParameter
            {
                CustomerLevelId = CustomerLevelId
            };
        }
    }
}
