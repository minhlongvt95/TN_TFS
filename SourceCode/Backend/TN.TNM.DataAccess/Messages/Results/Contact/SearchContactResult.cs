using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Messages.Results.Contact
{
    public class SearchContactResult : BaseResult
    {
        public List<LeadEntityModel> ContactList { get; set; }
    }
}
