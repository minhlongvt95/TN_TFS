using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Ward;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Ward
{
    public class GetAllWardByDistrictIdRequest : BaseRequest<GetAllWardByDistrictIdParameter>
    {
        public Guid DistrictId { get; set; }

        public override GetAllWardByDistrictIdParameter ToParameter()
        {
            return new GetAllWardByDistrictIdParameter()
            {
                DistrictId = DistrictId
            };
        }
    }
}
