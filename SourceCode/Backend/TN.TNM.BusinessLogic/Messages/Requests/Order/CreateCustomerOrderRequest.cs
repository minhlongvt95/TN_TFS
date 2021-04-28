using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class CreateCustomerOrderRequest : BaseRequest<CreateCustomerOrderParameter>
    {
        public CustomerOrderModel CustomerOrder { get; set; }
        public List<CustomerOrderDetailModel> CustomerOrderDetail { get; set; }
        public List<OrderCostDetailModel> OrderCostDetail { get; set; }
        public int TypeAccount { get; set; }
        public ContactModel Contact { get; set; }
        public Guid? QuoteId { get; set; }
        public override CreateCustomerOrderParameter ToParameter()
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
                    item.OrderProductDetailProductAttributeValue.ForEach(itemX =>
                    {
                        OrderProductDetailProductAttributeValueList.Add(itemX.ToEntity());
                    });
                    customerOrderDetailObject.OrderProductDetailProductAttributeValue = OrderProductDetailProductAttributeValueList;
                }
                ListcustomerOrderDetail.Add(customerOrderDetailObject);
            });
            List<OrderCostDetail> ListcustomerOrderCostDetail = new List<OrderCostDetail>();
            OrderCostDetail.ForEach(item =>
            {
                var orderCostDetailModelObj = new OrderCostDetail();
                orderCostDetailModelObj = item.ToEntity();
                ListcustomerOrderCostDetail.Add(orderCostDetailModelObj);
            });

            return new CreateCustomerOrderParameter
            {
                CustomerOrder = CustomerOrder.ToEntity(),
                CustomerOrderDetail = ListcustomerOrderDetail,
                OrderCostDetail = ListcustomerOrderCostDetail,
                Contact =Contact.ToEntity(),
                TypeAccount=TypeAccount,
                QuoteId= QuoteId,
                UserId =this.UserId
            };
        }
    }
}
