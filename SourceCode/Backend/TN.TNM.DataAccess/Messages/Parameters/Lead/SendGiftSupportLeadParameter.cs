using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class SendGiftSupportLeadParameter: BaseParameter
    {
        public string Title { get; set; }
        public int? GiftCustomerType1 { get; set; }
        public Guid? GiftTypeId1 { get; set; }
        public int? GiftTotal1 { get; set; }
        public int? GiftCustomerType2 { get; set; }
        public Guid? GiftTypeId2 { get; set; }
        public int? GiftTotal2 { get; set; }
        public Guid? CustomerId { get; set; }
    }
}
