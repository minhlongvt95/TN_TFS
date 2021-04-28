using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Product
{
    public class AddSerialNumberRequest: BaseRequest<AddSerialNumberParameter>
    {
        public Guid ProductId { get; set; }

        public override AddSerialNumberParameter ToParameter()
        {
            return new AddSerialNumberParameter
            {
                ProductId = ProductId
            };
        }
    }
}
