using System.Collections.Generic;
using Entities = TN.TNM.DataAccess.Databases.Entities;


namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetListCustomerOrderByIdCustomerIdResult:BaseResult
    {
        public List<Entities.CustomerOrder> listCustomerOrder { get; set; }
    }
}
