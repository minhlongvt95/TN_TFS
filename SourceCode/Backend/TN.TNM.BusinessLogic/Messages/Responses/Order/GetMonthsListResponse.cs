using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetMonthsListResponse : BaseResponse
    {
        public List<dynamic> monthOrderList { get; set; }
    }
}
