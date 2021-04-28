using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetAllCustomerOrderRequest : BaseRequest<GetAllCustomerOrderParameter>
    {
        public override GetAllCustomerOrderParameter ToParameter() => new GetAllCustomerOrderParameter()
        {
            CustomerName = CustomerName,
            OrderDateStart = OrderDateStart,
            OrderDateEnd = OrderDateEnd,
            OrderCode = OrderCode,
            ProductCode = ProductCode,
            Seller = Seller,
            Vat = Vat,
            Top3NewOrder= Top3NewOrder,
            ListOrganizationId = ListOrganizationId,
            Phone = Phone,
            UserId = UserId
        };
        public string OrderCode { get; set; }
        public string CustomerName { get; set; }
        public string ProductCode { get; set; }
        public Guid? Seller { get; set; }
        public List<Guid?> OrderStatusId { get; set; }
        public DateTime? OrderDateStart { get; set; }
        public DateTime? OrderDateEnd { get; set; }
        public int Vat { get; set; }
        public int? Top3NewOrder { get; set; }
        public List<Guid?> ListOrganizationId { get; set; }
        public string Phone { get; set; }
    }
}
