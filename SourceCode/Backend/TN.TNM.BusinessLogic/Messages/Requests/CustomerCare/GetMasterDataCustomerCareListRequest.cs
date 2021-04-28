using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class GetMasterDataCustomerCareListRequest : BaseRequest<GetMasterDataCustomerCareListParameter>
    {
        public override GetMasterDataCustomerCareListParameter ToParameter()
        {
            return new GetMasterDataCustomerCareListParameter
            {
                UserId = this.UserId
            };
        }
    }
}
