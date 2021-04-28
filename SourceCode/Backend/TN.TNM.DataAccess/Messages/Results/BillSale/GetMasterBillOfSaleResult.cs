﻿using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.BillSale;

namespace TN.TNM.DataAccess.Messages.Results.BillSale
{
    public class GetMasterBillOfSaleResult : BaseResult
    {
        public List<Databases.Entities.Product> ListProduct { get; set; }
        public List<Databases.Entities.Category> ListStatus { get; set; }
    }
}
