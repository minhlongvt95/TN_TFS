using System;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetConvertRateRequest : BaseRequest<GetConvertRateParameter>
    {
        public int? Month { get; set; }
        public int? Year { get; set; }

        public override GetConvertRateParameter ToParameter() => new GetConvertRateParameter
        {
            UserId = UserId,
            Month = Month,
            Year = Year
        };
    }
}
