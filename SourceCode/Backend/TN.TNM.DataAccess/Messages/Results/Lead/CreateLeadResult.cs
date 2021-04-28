using System;
using TN.TNM.DataAccess.Models.Email;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class CreateLeadResult : BaseResult
    {
        public Guid LeadId{get;set;}
        public string PicName { get; set; }
        public string Potential { get; set; }
        public string StatusName { get; set; }
        public DataAccess.Models.Email.SendEmailEntityModel SendEmailEntityModel { get; set; }
    }
}
