using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class CreateUpdateSerialResult : BaseResult
    {
        public List<SerialEntityModel> ListSerial { get; set; }
    }
}
