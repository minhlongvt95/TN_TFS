using System.Collections.Generic;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class SendEmailLeadResult : BaseResult
    {
        //public Guid QueueId { get; set; }
        public List<DataAccess.Databases.Entities.Contact> ListCustomerEmailIgnored { get; set; }
    }
}
