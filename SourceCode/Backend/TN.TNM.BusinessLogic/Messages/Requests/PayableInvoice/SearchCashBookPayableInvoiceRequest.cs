using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice
{
    public class SearchCashBookPayableInvoiceRequest: BaseRequest<SearchCashBookPayableInvoiceParameter>
    {
        public List<Guid> CreatedByIdList { get; set; }
        public DateTime? FromPaidDate { get; set; }
        public DateTime? ToPaidDate { get; set; }
        public List<Guid?> OrganizationList { get; set; }
        public override SearchCashBookPayableInvoiceParameter ToParameter()
        {
            return new SearchCashBookPayableInvoiceParameter()
            {
                UserId = UserId,
                CreatedByIdList = CreatedByIdList,
                FromPaidDate = FromPaidDate,
                ToPaidDate = ToPaidDate,
                OrganizationList = OrganizationList
            };
        }
    }
}
