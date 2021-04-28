using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class CreateUpdateSerialResponse : BaseResponse
    {
        public List<SerialEntityModel> ListSerial { get; set; }
    }
}
