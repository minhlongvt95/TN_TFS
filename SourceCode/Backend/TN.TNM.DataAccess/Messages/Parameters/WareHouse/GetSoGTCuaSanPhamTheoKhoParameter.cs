using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class GetSoGTCuaSanPhamTheoKhoParameter : BaseParameter
    {
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }
        public decimal QuantityRequest { get; set; }
    }
}
