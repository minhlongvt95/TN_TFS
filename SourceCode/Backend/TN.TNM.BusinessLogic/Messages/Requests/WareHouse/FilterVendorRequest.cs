using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class FilterVendorRequest : BaseRequest<FilterVendorParameter>
    {
        public override FilterVendorParameter ToParameter()
        {
            return new FilterVendorParameter { };
        }
    }
}
