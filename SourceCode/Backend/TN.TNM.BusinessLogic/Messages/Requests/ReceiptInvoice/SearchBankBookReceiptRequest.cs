using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice
{
    public class SearchBankBookReceiptRequest : BaseRequest<SearchBankBookReceiptParameter>
    {

        public DateTime? ToPaidDate { get; set; }
        public DateTime? FromPaidDate { get; set; }
        public List<Guid> BankAccountId { get; set; }
        public List<Guid> ListCreateById { get; set; }
        public override SearchBankBookReceiptParameter ToParameter()
        {
            return new SearchBankBookReceiptParameter()
            {
                UserId = UserId,
                BankAccountId=BankAccountId,
                FromPaidDate=FromPaidDate,
                ToPaidDate=ToPaidDate,
                ListCreateById = ListCreateById,
            };
        }

    }
}
