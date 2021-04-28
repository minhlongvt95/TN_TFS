using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Vendor
{
    public class ItemInvalidModel
    {
        public Guid? ProcurementRequestItemId { get; set; }
        public decimal? RemainQuantity { get; set; }
    }
}
