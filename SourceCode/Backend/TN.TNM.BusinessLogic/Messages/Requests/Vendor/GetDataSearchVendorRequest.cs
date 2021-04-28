using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class GetDataSearchVendorRequest: BaseRequest<GetDataSearchVendorParameter>
    {
        public override GetDataSearchVendorParameter ToParameter()
        {
            return new GetDataSearchVendorParameter()
            {
                UserId = UserId
            };
        }
    }
}
