using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Responses.Contact
{
    public class DeleteContactByIdResponse : BaseResponse
    {
        public List<CustomerOtherContactBusinessModel> ListContact { get; set; }
    }
}
