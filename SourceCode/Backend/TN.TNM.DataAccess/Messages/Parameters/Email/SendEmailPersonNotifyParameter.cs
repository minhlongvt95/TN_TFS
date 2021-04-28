using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Email
{
    public class SendEmailPersonNotifyParameter : BaseParameter
    {
        public List<Guid> NotifyId { get; set; }
        public string AccountCreate { get; set; }
        public string FullNameCreate { get; set; }
        public string RequestId { get; set; }
        public string FullNameRequest { get; set; }
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
