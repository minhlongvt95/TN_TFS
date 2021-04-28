using System;
using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice
{
    public class GetPayableInvoiceByIdRequest : BaseRequest<GetPayableInvoiceByIdParameter>
    {
        public Guid PayableInvoiceId { get; set; }
        
        public override GetPayableInvoiceByIdParameter ToParameter()
        {
            return new GetPayableInvoiceByIdParameter()
            {
                PayableInvoiceId = PayableInvoiceId
            };
        }
    }
}
