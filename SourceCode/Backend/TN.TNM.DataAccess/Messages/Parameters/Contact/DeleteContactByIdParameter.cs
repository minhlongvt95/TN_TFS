using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Contact
{
    public class DeleteContactByIdParameter : BaseParameter
    {
        public Guid ContactId { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
    }
}