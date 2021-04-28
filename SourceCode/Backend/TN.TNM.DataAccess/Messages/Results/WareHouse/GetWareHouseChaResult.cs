using System.Collections.Generic;
using Entities = TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetWareHouseChaResult : BaseResult
    {
        public List<Entities.Warehouse> listWareHouse { get; set; }
    }
}
