using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Email
{
    public class SendEmailAfterEditPicParameter : BaseParameter
    {
        public Guid PicId { get; set; }
        public Guid LeadId { get; set; }
        public string PotentialName { get; set; }
        public string StatusName { get; set; }
        public string PicName { get; set; }
        public string CurrentUrl { get; set; }
        public List<Guid> EmpCCIdList { get; set; }
    }
}
