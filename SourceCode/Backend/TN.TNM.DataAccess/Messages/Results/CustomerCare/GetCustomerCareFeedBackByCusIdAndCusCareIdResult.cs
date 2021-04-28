using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.DataAccess.Messages.Results.CustomerCare
{
    public class GetCustomerCareFeedBackByCusIdAndCusCareIdResult : BaseResult
    {
        public CustomerCareFeedBackEntityModel CustomerCareFeedBack { get; set; }
    }
}
