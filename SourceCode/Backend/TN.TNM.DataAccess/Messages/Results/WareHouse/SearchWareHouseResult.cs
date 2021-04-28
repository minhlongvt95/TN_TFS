using System.Collections.Generic;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class SearchWareHouseResult : BaseResult
    {
        public List<WareHouseEntityModel> listWareHouse { get; set; }
    }
}
