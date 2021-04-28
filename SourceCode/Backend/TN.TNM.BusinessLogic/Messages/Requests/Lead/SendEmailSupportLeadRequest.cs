using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Queue;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class SendEmailSupportLeadRequest : BaseRequest<SendEmailSupportLeadParameter>
    {
        //public QueueModel Queue { get; set; }
        public List<QueueModel> ListQueue { get; set; }

        public override SendEmailSupportLeadParameter ToParameter()
        {
            var parameter = new SendEmailSupportLeadParameter
            {
                //Queue = Queue.ToEntity(),
                ListQueue = new List<DataAccess.Databases.Entities.Queue>(),
                UserId = UserId
            };

            ListQueue.ForEach(item =>
            {
                parameter.ListQueue.Add(item.ToEntity());
            });

            return parameter;
        }
    }
}
