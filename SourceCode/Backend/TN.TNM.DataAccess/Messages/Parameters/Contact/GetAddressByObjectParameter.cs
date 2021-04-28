using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Contact
{
    public class GetAddressByObjectParameter : BaseParameter
    {
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
    }
}
