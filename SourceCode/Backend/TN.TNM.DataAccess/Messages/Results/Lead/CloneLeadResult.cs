using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class CloneLeadResult : BaseResult
    {
        public Guid ContactId { get; set; }
        public Guid LeadId { get; set; }
        public string PicName { get; set; }
        public string Potential { get; set; }
        public string StatusName { get; set; }
        public DataAccess.Models.Email.SendEmailEntityModel SendEmailEntityModel { get; set; }
    }
}
