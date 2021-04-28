using System;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class EditCustomerAdditionalRequest : BaseRequest<EditCustomerAdditionalParameter>
    {
        public Guid CustomerAdditionalInformationId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }

        public override EditCustomerAdditionalParameter ToParameter()
        {
            return new EditCustomerAdditionalParameter()
            {
                CustomerAdditionalInformationId = CustomerAdditionalInformationId,
                Question = Question,
                Answer = Answer,
                UserId = UserId
            };
        }
    }
}
