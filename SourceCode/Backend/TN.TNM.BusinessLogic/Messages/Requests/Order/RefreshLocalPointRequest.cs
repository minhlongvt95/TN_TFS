using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class RefreshLocalPointRequest : BaseRequest<RefreshLocalPointParameter>
    {
        public override RefreshLocalPointParameter ToParameter()
        {
            return new RefreshLocalPointParameter()
            {
                UserId = UserId
            };
        }
    }
}
