using System;
using TN.TNM.DataAccess.Messages.Parameters.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Email
{
    public class SendEmailPersonApproveRequest : BaseRequest<SendEmailPersonApproveParameter>
    {
        public Guid ApproveId { get; set; }
        public string RequestId { get; set; }
        public string FullNameRequest { get; set; }
        public string ActiveRequest { get; set; }
        public string AccountApprove { get; set; }
        public string FullNameApprove { get; set; }
        public string DateCreate { get; set; }
        public string RequestType { get; set; }
        public string DateStart { get; set; }
        public string CaStart { get; set; }
        public string DateEnd { get; set; }
        public string CaEnd { get; set; }
        public string Note { get; set; }
        public string ListFullNameNotify { get; set; }
        public Guid RequestEmployeeId { get; set; }

        public override SendEmailPersonApproveParameter ToParameter()
        {
            return new SendEmailPersonApproveParameter()
            {
                ApproveId = ApproveId,
                RequestId = RequestId,
                FullNameRequest = FullNameRequest,
                ActiveRequest = ActiveRequest,
                AccountApprove = AccountApprove,
                FullNameApprove = FullNameApprove,
                DateCreate = DateCreate,
                RequestType = RequestType,
                DateStart = DateStart,
                CaStart = CaStart,
                DateEnd = DateEnd,
                CaEnd = CaEnd,
                Note = Note,
                ListFullNameNotify = ListFullNameNotify,
                RequestEmployeeId = RequestEmployeeId
            };
        }
    }
}
