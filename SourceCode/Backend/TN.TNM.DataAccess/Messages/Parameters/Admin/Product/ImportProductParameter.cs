using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class ImportProductParameter : BaseParameter
    {
        public List<Models.Product.ProductEntityModel> ListProduct { get; set; }
    }
}
