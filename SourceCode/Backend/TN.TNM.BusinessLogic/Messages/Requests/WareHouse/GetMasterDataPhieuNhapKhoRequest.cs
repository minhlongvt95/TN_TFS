using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetMasterDataPhieuNhapKhoRequest : BaseRequest<GetMasterDataPhieuNhapKhoParameter>
    {
        public override GetMasterDataPhieuNhapKhoParameter ToParameter()
        {
            return new GetMasterDataPhieuNhapKhoParameter()
            {
                UserId = UserId
            };
        }
    }
}
