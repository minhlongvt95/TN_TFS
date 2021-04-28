using TN.TNM.DataAccess.Messages.Parameters.Admin.OrderStatus;
using TN.TNM.DataAccess.Messages.Results.Admin.OrderStatus;
//using TNT.N8.DataAccess.Messages.Parameters.Category;

//using TNT.N8.DataAccess.Messages.Results.Categorys;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IOrderStatusDataAccess
    {
        /// <summary>
        /// Get all order status
        /// </summary>
        /// <param name="parameter">parameter</param>
        /// <returns></returns>
        GetAllOrderStatusResult GetAllOrderStatus(GetAllOrderStatusParameter parameter);
        /// <summary>
        /// Get Order Status By ID
        /// </summary>
        /// <param name="parameter">parameter</param>
        /// <returns></returns>
        GetOrderStatusByIDResult GetOrderStatusByID(GetOrderStatusByIDParameter parameter);


    }
}
