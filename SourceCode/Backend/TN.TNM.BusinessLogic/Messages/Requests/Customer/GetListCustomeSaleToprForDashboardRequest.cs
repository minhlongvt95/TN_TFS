using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetListCustomeSaleToprForDashboardRequest : BaseRequest<GetListCustomeSaleToprForDashboardParameter>
    {
        public string KeyName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public override GetListCustomeSaleToprForDashboardParameter ToParameter()
        {
            return new GetListCustomeSaleToprForDashboardParameter()
            {
                KeyName = KeyName,
                Month = Month,
                Year = Year,
                UserId = UserId
            };
        }
    }
}
