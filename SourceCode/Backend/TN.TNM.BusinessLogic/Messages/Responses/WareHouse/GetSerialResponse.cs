using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class GetSerialResponse:BaseResponse
    {
        public List<SerialModel> lstSerial { get; set; }
    }
}
