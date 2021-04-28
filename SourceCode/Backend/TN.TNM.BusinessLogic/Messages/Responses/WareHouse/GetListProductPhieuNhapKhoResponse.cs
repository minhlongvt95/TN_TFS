using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class GetListProductPhieuNhapKhoResponse : BaseResponse
    {
        public List<ProductEntityModel> ListProduct { get; set; }
    }
}
