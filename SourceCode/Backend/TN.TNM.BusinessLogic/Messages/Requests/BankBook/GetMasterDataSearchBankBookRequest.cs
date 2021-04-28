using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.BankBook;

namespace TN.TNM.BusinessLogic.Messages.Requests.BankBook
{
    public class GetMasterDataSearchBankBookRequest : BaseRequest<GetMasterDataSearchBankBookParameter>
    {
        public override GetMasterDataSearchBankBookParameter ToParameter()
        {
            return new GetMasterDataSearchBankBookParameter
            {
                UserId = UserId,
            };
        }
    }
}
