namespace TN.TNM.DataAccess.Messages.Results.Admin.OrderStatus
{
    public class GetOrderStatusByIDResult : BaseResult
    {
      public Databases.Entities.OrderStatus orderStatus { get; set; }
    }
}
