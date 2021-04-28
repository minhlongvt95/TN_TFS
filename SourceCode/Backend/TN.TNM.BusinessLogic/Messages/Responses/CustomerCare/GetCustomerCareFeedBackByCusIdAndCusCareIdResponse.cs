using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Responses.CustomerCare
{
    public class GetCustomerCareFeedBackByCusIdAndCusCareIdResponse : BaseResponse
    {
        public CustomerCareFeedBackEntityModel CustomerCareFeedBack { get; set; }
    }
}
