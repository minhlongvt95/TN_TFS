using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class GetTotalInteractiveRequest : BaseRequest<GetTotalInteractiveParameter>
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public override GetTotalInteractiveParameter ToParameter()
        {
            return new GetTotalInteractiveParameter
            {
                Month = Month,
                Year = Year
            };
        }
    }
}
