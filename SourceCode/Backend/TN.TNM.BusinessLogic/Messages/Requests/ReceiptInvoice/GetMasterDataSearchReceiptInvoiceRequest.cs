using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice
{
    public class GetMasterDataSearchReceiptInvoiceRequest : BaseRequest<GetMasterDataSearchReceiptInvoiceParameter>
    {
        public override GetMasterDataSearchReceiptInvoiceParameter ToParameter()
        {
            return new GetMasterDataSearchReceiptInvoiceParameter
            {
                UserId = UserId
            };
        }
    }
}
