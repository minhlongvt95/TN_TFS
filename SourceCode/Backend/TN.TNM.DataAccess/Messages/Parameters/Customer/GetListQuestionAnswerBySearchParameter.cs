using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class GetListQuestionAnswerBySearchParameter : BaseParameter
    {
        public Guid CustomerId { get; set; }
        public string TextSearch { get; set; }
    }
}
