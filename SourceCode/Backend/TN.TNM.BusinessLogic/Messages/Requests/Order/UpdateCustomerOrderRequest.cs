using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class UpdateCustomerOrderRequest : BaseRequest<UpdateCustomerOrderParameter>
    {
        public CustomerOrderModel CustomerOrder { get; set; }
        public List<CustomerOrderDetailModel> CustomerOrderDetail { get; set; }
        public List<OrderCostDetailModel> OrderCostDetail { get; set; }
        public int TypeAccount { get; set; }

        public override UpdateCustomerOrderParameter ToParameter()
        {
            var OrderProductDetailProductAttributeValue = new OrderProductDetailProductAttributeValue();
            List<CustomerOrderDetail> ListcustomerOrderDetail = new List<CustomerOrderDetail>();
            CustomerOrderDetail.ForEach(item =>
            {
                var customerOrderDetailObject = new CustomerOrderDetail();
                customerOrderDetailObject = item.ToEntity();
                List<OrderProductDetailProductAttributeValue> OrderProductDetailProductAttributeValueList = new List<OrderProductDetailProductAttributeValue>();
                if (item.OrderProductDetailProductAttributeValue != null)
                {
                    item.OrderProductDetailProductAttributeValue.ForEach(itemX => { OrderProductDetailProductAttributeValueList.Add(itemX.ToEntity()); });
                    customerOrderDetailObject.OrderProductDetailProductAttributeValue = OrderProductDetailProductAttributeValueList;
                }
                ListcustomerOrderDetail.Add(customerOrderDetailObject);
            });

            List<OrderCostDetail> ListOrderCostDetail = new List<OrderCostDetail>();
            OrderCostDetail.ForEach(item =>
            {
                var orderCostDetailObject = new OrderCostDetail();
                orderCostDetailObject = item.ToEntity();
                ListOrderCostDetail.Add(orderCostDetailObject);
            });

            return new UpdateCustomerOrderParameter
            {
                CustomerOrder = CustomerOrder.ToEntity(),
                CustomerOrderDetail = ListcustomerOrderDetail,
                OrderCostDetail = ListOrderCostDetail,
                TypeAccount = TypeAccount,
                UserId=this.UserId
            };
        }
    }
}
