using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class CreateOrderServiceRequest : BaseRequest<CreateOrderServiceParameter>
    {
        public CustomerOrderModel CustomerOrder { get; set; }
        public List<CustomerOrderDetailModel> ListCustomerOrderDetail { get; set; }
        public List<Guid> ListLocalPointId { get; set; }

        public override CreateOrderServiceParameter ToParameter()
        {
            List<CustomerOrderDetail> listcustomerOrderDetail = new List<CustomerOrderDetail>();
            ListCustomerOrderDetail.ForEach(item =>
            {
                var customerOrderDetailObject = new CustomerOrderDetail();
                customerOrderDetailObject = item.ToEntity();
                List<OrderProductDetailProductAttributeValue> OrderProductDetailProductAttributeValueList =
                    new List<OrderProductDetailProductAttributeValue>();
                if (item.OrderProductDetailProductAttributeValue != null)
                {
                    item.OrderProductDetailProductAttributeValue.ForEach(itemX =>
                    {
                        OrderProductDetailProductAttributeValueList.Add(itemX.ToEntity());
                    });
                    customerOrderDetailObject.OrderProductDetailProductAttributeValue =
                        OrderProductDetailProductAttributeValueList;
                }

                listcustomerOrderDetail.Add(customerOrderDetailObject);
            });

            return new CreateOrderServiceParameter()
            {
                UserId = UserId,
                CustomerOrder = CustomerOrder.ToEntity(),
                ListCustomerOrderDetail = listcustomerOrderDetail,
                ListLocalPointId = ListLocalPointId
            };
        }
    }
}
