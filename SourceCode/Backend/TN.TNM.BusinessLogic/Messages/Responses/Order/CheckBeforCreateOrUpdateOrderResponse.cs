using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class CheckBeforCreateOrUpdateOrderResponse : BaseResponse
    {
        public bool isCheckMaxDebt { get; set; }
    }
}
