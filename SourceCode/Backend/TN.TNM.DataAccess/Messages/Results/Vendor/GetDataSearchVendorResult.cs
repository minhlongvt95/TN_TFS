﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetDataSearchVendorResult: BaseResult
    {
        public List<Databases.Entities.Category> ListVendorGroup { get; set; }
    }
}
