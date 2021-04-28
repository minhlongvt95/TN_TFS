using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class CheckBeforCreateOrUpdateOrderParameter : BaseParameter
    {
        public Guid CustomerId { get; set; }
        public decimal MaxDebt { get; set; }
        public decimal AmountOrder { get; set; }
    }
}
