﻿using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class ChangeCustomerStatusToDeleteParameter : BaseParameter
    {
        public Guid CustomerId { get; set; }
    }
}
