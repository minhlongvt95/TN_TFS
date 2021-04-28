using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.CashBook;

namespace TN.TNM.BusinessLogic.Messages.Requests.CashBook
{
    public class GetDataSearchCashBookRequest : BaseRequest<GetDataSearchCashBookParameter>
    {
        public override GetDataSearchCashBookParameter ToParameter()
        {
            return new GetDataSearchCashBookParameter
            {
                UserId = UserId
            };
        }
    }
}
