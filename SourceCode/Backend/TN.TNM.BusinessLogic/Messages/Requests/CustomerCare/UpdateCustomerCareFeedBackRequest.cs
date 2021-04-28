using TN.TNM.BusinessLogic.Models.CustomerCare;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class UpdateCustomerCareFeedBackRequest : BaseRequest<UpdateCustomerCareFeedBackParameter>
    {
        public CustomerCareFeedBackModel CustomerCareFeedBack { get; set; }

        public override UpdateCustomerCareFeedBackParameter ToParameter()
        {
            return new UpdateCustomerCareFeedBackParameter
            {
                CustomerCareFeedBack = this.CustomerCareFeedBack.ToEntity(),
                UserId = this.UserId
            };
        }
    }
}
