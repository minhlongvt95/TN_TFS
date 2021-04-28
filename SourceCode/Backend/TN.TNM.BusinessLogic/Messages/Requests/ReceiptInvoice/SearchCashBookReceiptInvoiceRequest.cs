using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice
{
    public class SearchCashBookReceiptInvoiceRequest : BaseRequest<SearchCashBookReceiptInvoiceParameter>
    {
        public List<Guid> CreateById { get; set; }
        public DateTime? ReceiptDateFrom { get; set; }
        public DateTime? ReceiptDateTo { get; set; }
        public List<Guid?> OrganizationList { get; set; }
        public override SearchCashBookReceiptInvoiceParameter ToParameter()
        {
            return new SearchCashBookReceiptInvoiceParameter
            {
                CreateById = this.CreateById,
                ReceiptDateFrom = this.ReceiptDateFrom,
                ReceiptDateTo = this.ReceiptDateTo,
                UserId = this.UserId,
                OrganizationList = OrganizationList
            };
        }
    }
}
