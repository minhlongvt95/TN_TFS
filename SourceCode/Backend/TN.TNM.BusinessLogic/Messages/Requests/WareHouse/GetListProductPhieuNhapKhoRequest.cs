using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetListProductPhieuNhapKhoRequest : BaseRequest<GetListProductPhieuNhapKhoParameter>
    {
        public override GetListProductPhieuNhapKhoParameter ToParameter()
        {
            return new GetListProductPhieuNhapKhoParameter()
            {
                UserId = UserId
            };
        }
    }
}
