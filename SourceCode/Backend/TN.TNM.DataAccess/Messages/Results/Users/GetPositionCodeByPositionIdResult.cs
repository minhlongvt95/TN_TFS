using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Users
{
    public class GetPositionCodeByPositionIdResult : BaseResult
    {
        public string PositionCode { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public List<dynamic> lstResult { get; set; }
    }
}
