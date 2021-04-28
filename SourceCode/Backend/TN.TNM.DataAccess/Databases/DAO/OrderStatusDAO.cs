using System;
using System.Linq;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.OrderStatus;
using TN.TNM.DataAccess.Messages.Results.Admin.OrderStatus;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class OrderStatusDAO : BaseDAO, IOrderStatusDataAccess
    {
        public OrderStatusDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetAllOrderStatusResult GetAllOrderStatus(GetAllOrderStatusParameter parameter)
        {
            try
            {
                var listOrderStatus = context.OrderStatus.ToList();
                return new GetAllOrderStatusResult
                {
                    listOrderStatus = listOrderStatus,
                    Message = "Success",
                    Status=true
                };
                
            }
            catch (Exception ex)
            {
                return new GetAllOrderStatusResult
                {
                    Message = ex.ToString(),
                    Status = false
                };
            }
        }

        public GetOrderStatusByIDResult GetOrderStatusByID(GetOrderStatusByIDParameter parameter)
        {
            try
            {
                var OrderStatusobject = context.OrderStatus.Where(item => item.OrderStatusId == parameter.OderStatusId).FirstOrDefault();
                return new GetOrderStatusByIDResult
                {
                    orderStatus = OrderStatusobject,
                    Message = "Success",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new GetOrderStatusByIDResult
                {
                    Message = ex.ToString(),
                    Status = false
                };
            }
        }

    }
}
