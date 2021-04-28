using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class ImportPriceProductParamter : BaseParameter
    {
        public List<PriceProductEntityModel> ListPriceProduct { get; set; }
    }
}
