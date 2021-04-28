using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class EditLeadStatusByIdParameter : BaseParameter
    {
        public Guid LeadId;
        public string LeadStatusCode;
    }
}
