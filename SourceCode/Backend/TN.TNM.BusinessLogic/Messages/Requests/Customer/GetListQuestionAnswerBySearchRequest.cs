using System;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetListQuestionAnswerBySearchRequest : BaseRequest<GetListQuestionAnswerBySearchParameter>
    {
        public Guid CustomerId { get; set; }
        public string TextSearch { get; set; }

        public override GetListQuestionAnswerBySearchParameter ToParameter()
        {
            return new GetListQuestionAnswerBySearchParameter()
            {
                CustomerId = CustomerId,
                TextSearch = TextSearch,
                UserId = UserId
            };
        }
    }
}
