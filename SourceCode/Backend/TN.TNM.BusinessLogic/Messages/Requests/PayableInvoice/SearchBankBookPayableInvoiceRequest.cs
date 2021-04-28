using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice
{
    public class SearchBankBookPayableInvoiceRequest : BaseRequest<SearchBankBookPayableInvoiceParameter>
    {
        public DateTime? ToPaidDate { get; set; }
        public DateTime? FromPaidDate { get; set; }
        public List<Guid> BankAccountId { get; set; }
        public List<Guid> ListCreateById { get; set; }
        public override SearchBankBookPayableInvoiceParameter ToParameter()
        {
            return new SearchBankBookPayableInvoiceParameter() {
                UserId = UserId,
                ToPaidDate = ToPaidDate,
                FromPaidDate = FromPaidDate,
                BankAccountId = BankAccountId,
                ListCreateById = ListCreateById
            };
        }
    }
}
