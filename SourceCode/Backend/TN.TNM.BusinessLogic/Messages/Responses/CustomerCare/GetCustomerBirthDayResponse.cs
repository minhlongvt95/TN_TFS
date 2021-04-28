using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Responses.CustomerCare
{
    public class GetCustomerBirthDayResponse : BaseResponse
    {
        public List<GetCustomerBirthDayModel> ListBirthDay { get; set; }
    }
}
