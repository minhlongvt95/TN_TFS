using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class CreateUpdateSerialParameter : BaseParameter
    {
        public List<SerialEntityModel> ListSerial { get; set; }
        public Guid WarehouseId { get; set; }
        public Guid ProductId { get; set; }
    }
}
