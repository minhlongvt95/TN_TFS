﻿using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class InStockReportResult:BaseResult
    {
        public List<InStockEntityModel> lstResult { get; set; }
    }
}
