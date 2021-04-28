using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class GetWareHouseChaResponse : BaseResponse
    {
        public List<WareHouseModel> listWareHouse { get; set; }
    }
}
