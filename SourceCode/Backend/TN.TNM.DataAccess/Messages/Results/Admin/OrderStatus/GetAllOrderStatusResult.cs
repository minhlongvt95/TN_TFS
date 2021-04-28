using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Admin.OrderStatus
{
    public class GetAllOrderStatusResult: BaseResult
    {
      public List<Databases.Entities.OrderStatus> listOrderStatus { get; set; }
    }
}
