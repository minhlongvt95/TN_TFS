using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class SendGiftSupportLeadRequest : BaseRequest<SendGiftSupportLeadParameter>
    {
        public string Title { get; set; }
        public int? GiftCustomerType1 { get; set; }
        public Guid? GiftTypeId1 { get; set; }
        public int? GiftTotal1 { get; set; }
        public int? GiftCustomerType2 { get; set; }
        public Guid? GiftTypeId2 { get; set; }
        public int? GiftTotal2 { get; set; }
        public Guid CustomerId { get; set; }

        public override SendGiftSupportLeadParameter ToParameter()
        {
            return new SendGiftSupportLeadParameter
            {
                Title = Title,
                GiftCustomerType1 = GiftCustomerType1,
                GiftTypeId1 = GiftTypeId1,
                GiftTotal1 = GiftTotal1,
                GiftCustomerType2 = GiftCustomerType2,
                GiftTypeId2 = GiftTypeId2,
                GiftTotal2 = GiftTotal2,
                CustomerId = CustomerId,
                UserId = UserId
            };
        }
    }
}
