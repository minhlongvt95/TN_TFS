using System;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class CreateCustomerAdditionalRequest : BaseRequest<CreateCustomerAdditionalParameter>
    {
        public Guid? CustomerAdditionalInformationId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public Guid CustomerId { get; set; }

        public override CreateCustomerAdditionalParameter ToParameter()
        {
            return new CreateCustomerAdditionalParameter()
            {
                CustomerAdditionalInformationId = CustomerAdditionalInformationId,
                Question = Question,
                Answer = Answer,
                CustomerId = CustomerId,
                UserId = UserId
            };
        }
    }
}
