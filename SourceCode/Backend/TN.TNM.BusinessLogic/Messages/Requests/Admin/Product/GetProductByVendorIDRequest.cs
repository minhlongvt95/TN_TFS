using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Product
{
    public class GetProductByVendorIDRequest : BaseRequest<GetProductByVendorIDParameter>
    {

        public Guid VendorId { get; set; }

        public override GetProductByVendorIDParameter ToParameter()
        {

            return new GetProductByVendorIDParameter
            {
                VendorId= VendorId,
                UserId=this.UserId
            };
        }
    }
}
