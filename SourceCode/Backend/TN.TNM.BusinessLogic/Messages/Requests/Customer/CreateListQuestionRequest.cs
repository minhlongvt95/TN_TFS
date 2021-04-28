using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class CreateListQuestionRequest : BaseRequest<CreateListQuestionParameter>
    {
        public Guid CustomerId { get; set; }

        public override CreateListQuestionParameter ToParameter()
        {
            return new CreateListQuestionParameter()
            {
                CustomerId = CustomerId,
                UserId = UserId
            };
        }
    }
}
