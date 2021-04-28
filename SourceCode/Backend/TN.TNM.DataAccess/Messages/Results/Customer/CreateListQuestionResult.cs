using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class CreateListQuestionResult : BaseResult
    {
        public List<CustomerAdditionalInformation> ListCustomerAdditionalInformation { get; set; }
    }
}
