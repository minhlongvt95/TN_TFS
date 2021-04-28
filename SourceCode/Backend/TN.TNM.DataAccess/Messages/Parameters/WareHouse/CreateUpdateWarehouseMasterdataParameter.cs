using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class CreateUpdateWarehouseMasterdataParameter: BaseParameter
    {
        public Guid? WarehouseId { get; set; }
    }
}
