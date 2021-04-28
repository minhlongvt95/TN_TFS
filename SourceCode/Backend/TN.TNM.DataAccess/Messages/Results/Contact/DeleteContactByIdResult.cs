using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Results.Contact
{
    public class DeleteContactByIdResult : BaseResult
    {
        public List<CustomerOtherContactModel> ListContact { get; set; }
    }
}
