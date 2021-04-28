using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Contact
{
    public class GetContactByIdParameter : BaseParameter
    {
        public Guid ContactId { get; set; }
}
}
