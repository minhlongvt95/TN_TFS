﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class CreateCostParameter : BaseParameter
    {
        public string CostCode { get; set; }
        public string CostName { get; set; }
        public Guid? OrganzationId { get; set; }
        public Guid? StatusId { get; set; }
    }
}
