using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.DashboardRequest
{
    public class GetAllRequestResult : BaseResult
    {
        public List<RequestDetail> RequestList { get; set; }
    }

    public class RequestDetail
    {
        public Guid? RequestId { get; set; }
        public Guid? CreateEmployeeId { get; set; }
        public string CreateEmployeeName { get; set; }
        public string RequestCode { get; set; }
        public string RequestContent { get; set; }
        public Guid? RequestTypeId { get; set; }
        public string RequestTypeName { get; set; }
        public int? Month { get; set; }
    }
}
