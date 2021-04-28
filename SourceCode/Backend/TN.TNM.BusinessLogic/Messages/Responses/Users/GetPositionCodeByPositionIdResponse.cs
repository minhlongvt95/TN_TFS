using System;
using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Users
{
    public class GetPositionCodeByPositionIdResponse : BaseResponse
    {
        public string PositionCode { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public List<dynamic> lstResult { get; set; }
    }
}
