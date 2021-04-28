using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Product
{
    public class GetDataCreateUpdateBOMResponse: BaseResponse
    {
        public List<DataAccess.Models.Product.ProductEntityModel> ListProduct { get; set; }
    }
}
