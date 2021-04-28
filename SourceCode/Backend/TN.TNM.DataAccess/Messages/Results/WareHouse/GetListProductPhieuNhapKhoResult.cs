using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetListProductPhieuNhapKhoResult : BaseResult
    {
        public List<ProductEntityModel> ListProduct { get; set; }
    }
}
