using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class DownloadTemplateCustomerRequest : BaseRequest<DownloadTemplateCustomerParameter>
    {
        public int CustomerType { get; set; }

        public override DownloadTemplateCustomerParameter ToParameter()
        {
            return new DownloadTemplateCustomerParameter
            {
                CustomerType = this.CustomerType,
                UserId = UserId
            };
        }
    }
}
