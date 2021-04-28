using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class SetPersonalInChangeRequest: BaseRequest<SetPersonalInChangeParameter>
    {
        public Guid? CustomerId { get; set; }

        public override SetPersonalInChangeParameter ToParameter()
        {
            return new SetPersonalInChangeParameter()
            {
                UserId = UserId,
                CustomerId = CustomerId
            };
        }
    }
}
