using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class UpdateCustomerServiceRequest : BaseRequest<UpdateCustomerServiceParameter>
    {
        public Guid OrderId { get; set; }
        public List<CustomerOrderDetailModel> ListCustomerOrderDetail { get; set; }

        public override UpdateCustomerServiceParameter ToParameter()
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

            return new UpdateCustomerServiceParameter()
            {
                UserId = UserId,
                OrderId = OrderId,
                ListCustomerOrderDetail = listcustomerOrderDetail
            };
        }
    }
}
