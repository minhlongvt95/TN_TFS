using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class CreateListQuestionParameter : BaseParameter
    {
        public Guid CustomerId { get; set; }
    }
}
