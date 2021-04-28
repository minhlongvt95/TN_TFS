using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Contact
{
    public class GetContactByObjectIdParameter : BaseParameter
    {
        public Guid ObjectId { get; set; }
}
}
