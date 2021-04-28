using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class ProfitAccordingCustomersRequest : BaseRequest<ProfitAccordingCustomersParameter>
    {
        public string OrderCode { get; set; }
        public string CustomerName { get; set; }
        public List<Guid> ListStatusId { get; set; }
        public Guid? QuoteId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? Seller { get; set; }
        public override ProfitAccordingCustomersParameter ToParameter()
        {
            return new ProfitAccordingCustomersParameter
            {
                OrderCode = OrderCode,
                CustomerName = CustomerName,
                ListStatusId = ListStatusId,
                QuoteId = QuoteId,
                FromDate = FromDate,
                ToDate = ToDate,
                ProductId = ProductId,
                Seller = Seller,
                UserId =this.UserId
            };
        }
    }
}
