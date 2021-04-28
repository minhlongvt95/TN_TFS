using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.District;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.District
{
    public class GetAllDistrictByProvinceIdRequest : BaseRequest<GetAllDistrictByProvinceIdParameter>
    {
        public Guid ProvinceId { get; set; }
        public override GetAllDistrictByProvinceIdParameter ToParameter()
        {
            return new GetAllDistrictByProvinceIdParameter()
            {
                ProviceId = ProvinceId,
                UserId = UserId
            };
        }
    }
}
