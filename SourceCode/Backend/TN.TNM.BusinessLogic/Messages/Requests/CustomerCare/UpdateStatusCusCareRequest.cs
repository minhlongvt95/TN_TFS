using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class UpdateStatusCusCareRequest : BaseRequest<UpdateStatusCusCareParameter>
    {
        public Guid CustomerCareId { get; set; }
        public Guid StatusId { get; set; }
        public override UpdateStatusCusCareParameter ToParameter()
        {
            return new UpdateStatusCusCareParameter
            {
                StatusId = this.StatusId,
                CustomerCareId = this.CustomerCareId,
                UserId = this.UserId
            };
        }
    }
}
