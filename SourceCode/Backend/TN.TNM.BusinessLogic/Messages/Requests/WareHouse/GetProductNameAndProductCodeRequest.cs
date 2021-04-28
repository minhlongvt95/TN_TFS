using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetProductNameAndProductCodeRequest : BaseRequest<GetProductNameAndProductCodeParameter>
    {
        public string Query { get; set; }

        public override GetProductNameAndProductCodeParameter ToParameter()
        {
            return new GetProductNameAndProductCodeParameter
            {
                Query=this.Query
            };
}
    }
}
