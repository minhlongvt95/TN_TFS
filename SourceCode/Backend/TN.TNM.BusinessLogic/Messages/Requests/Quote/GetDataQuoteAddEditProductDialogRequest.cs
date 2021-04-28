using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class GetDataQuoteAddEditProductDialogRequest : BaseRequest<GetDataQuoteAddEditProductDialogParameter>
    {
        public override GetDataQuoteAddEditProductDialogParameter ToParameter()
        {
            return new GetDataQuoteAddEditProductDialogParameter()
            {
                UserId = UserId
            };
        }
    }
}
