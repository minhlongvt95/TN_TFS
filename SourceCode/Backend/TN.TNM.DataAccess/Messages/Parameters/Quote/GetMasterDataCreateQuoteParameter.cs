using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class GetMasterDataCreateQuoteParameter : BaseParameter
    {
        public Guid? ObjectId { get; set; }
        public string ObjectType { get; set; }
    }
}
