using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Email
{
    public class SendEmailPersonCreateParameter : BaseParameter
    {
        public Guid CreateId { get; set; }
        public string FullNameCreate { get; set; }
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
    }
}
