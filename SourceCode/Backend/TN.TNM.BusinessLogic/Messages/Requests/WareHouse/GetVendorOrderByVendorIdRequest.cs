using System;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetVendorOrderByVendorIdRequest : BaseRequest<GetVendorOrderByVendorIdParameter>
    {
        public Guid VendorId { get; set; }
        public override GetVendorOrderByVendorIdParameter ToParameter()
        {
            return new GetVendorOrderByVendorIdParameter()
            {
                UserId = UserId,
                VendorId = VendorId
            };
        }
    }
}
