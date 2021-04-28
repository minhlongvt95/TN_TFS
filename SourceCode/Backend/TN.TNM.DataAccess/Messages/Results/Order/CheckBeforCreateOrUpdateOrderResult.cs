using System;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class CheckBeforCreateOrUpdateOrderResult : BaseResult
    {
        public bool isCheckMaxDebt { get; set; }
    }
}
