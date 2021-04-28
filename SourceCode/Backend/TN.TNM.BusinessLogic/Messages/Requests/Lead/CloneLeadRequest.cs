using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class CloneLeadRequest : BaseRequest<CloneLeadParameter>
    {
        public Guid LeadId { get; set; }

        public override CloneLeadParameter ToParameter()
        {
            return new CloneLeadParameter
            {
                LeadId = LeadId,
                UserId = UserId
            };
        }
    }
}
