using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetMasterDataListPhieuNhapKhoRequest : BaseRequest<GetMasterDataListPhieuNhapKhoParameter>
    {
        public override GetMasterDataListPhieuNhapKhoParameter ToParameter()
        {
            return new GetMasterDataListPhieuNhapKhoParameter()
            {
                UserId = UserId
            };
        }
    }
}
