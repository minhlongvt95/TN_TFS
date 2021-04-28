using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class SearchOrderRequest : BaseRequest<SearchOrderParameter>
    {
        public string OrderCode { get; set; }
        public string CustomerName { get; set; }
        public List<Guid> ListStatusId { get; set; }
        public string Phone { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Vat { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? QuoteId { get; set; }
        public Guid? ContractId { get; set; }

        public override SearchOrderParameter ToParameter()
        {
            return new SearchOrderParameter()
            {
                UserId = UserId,
                OrderCode = OrderCode,
                CustomerName = CustomerName,
                ListStatusId = ListStatusId,
                Phone = Phone,
                FromDate = FromDate,
                ToDate = ToDate,
                Vat = Vat,
                ProductId = ProductId,
                QuoteId = QuoteId,
                ContractId = ContractId
            };
        }
    }
}
