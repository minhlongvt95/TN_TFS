using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.BillSale;

namespace TN.TNM.BusinessLogic.Messages.Requests.BillSale
{
    public class SearchBillOfSaleRequest : BaseRequest<SearchBillOfSaleParameter>
    {
        public string BillOfSaleCode { get; set; }
        public string OrderCode { get; set; }
        public string CustomerName { get; set; }
        public List<Guid> ListStatusId { get; set; }
        public List<Guid> ListProductId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public override SearchBillOfSaleParameter ToParameter()
        {
            return new SearchBillOfSaleParameter()
            {
                BillOfSaleCode = BillOfSaleCode,
                OrderCode = OrderCode,
                CustomerName = CustomerName,
                ListStatusId = ListStatusId,
                ListProductId = ListProductId,
                FromDate = FromDate,
                ToDate = ToDate,
                UserId =UserId
            };
        }
    }
}
