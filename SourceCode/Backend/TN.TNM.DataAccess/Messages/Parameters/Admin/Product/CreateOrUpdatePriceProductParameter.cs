using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class CreateOrUpdatePriceProductParameter : BaseParameter
    {
        public Models.Product.PriceProductEntityModel PriceProduct { get; set; }
    }
}
