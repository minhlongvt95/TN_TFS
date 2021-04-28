using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Email
{
    public class SendEmailAfterEditPicRequest : BaseRequest<SendEmailAfterEditPicParameter>
    {
        public Guid PicId { get; set; }
        public Guid LeadId { get; set; }
        public string CurrentUrl { get; set; }
        public string PotentialName { get; set; }
        public string StatusName { get; set; }
        public string PicName { get; set; }
        public List<Guid> EmpCCIdList { get; set; }
        public override SendEmailAfterEditPicParameter ToParameter()
        {
            return new SendEmailAfterEditPicParameter()
            {
                UserId = UserId,
                PicId = PicId,
                LeadId = LeadId,
                CurrentUrl = CurrentUrl,
                EmpCCIdList = EmpCCIdList,
                PicName = PicName,
                PotentialName = PotentialName,
                StatusName = StatusName
            };
        }
    }
}
