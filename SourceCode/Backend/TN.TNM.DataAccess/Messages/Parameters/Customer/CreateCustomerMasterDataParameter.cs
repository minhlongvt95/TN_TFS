﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class CreateCustomerMasterDataParameter: BaseParameter
    {
        public Guid EmployeeId { get; set; }
    }
}
