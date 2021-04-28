using System;
using TN.TNM.DataAccess.Models.Email;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class CreateLeadResponse : BaseResponse
    {
        public Guid LeadId { get; set; }
        public Guid ContactId { get; set; }
        public string PicName { get; set; }
        public string Potential { get; set; }
        public string StatusName { get; set; }
        public DataAccess.Models.Email.SendEmailEntityModel SendEmailEntityModel { get; set; }
    }
}
