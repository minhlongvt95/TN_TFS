using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetDataCreateUpdateBOMResult: BaseResult
    {
        public List<DataAccess.Models.Product.ProductEntityModel> ListProduct { get; set; }
    }
}
