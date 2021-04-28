using System;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class SendQuickGiftRequest : BaseRequest<SendQuickGiftParameter>
    {
        public string Title { get; set; }
        public int? GiftCustomerType1 { get; set; }
        public Guid? GiftTypeId1 { get; set; }
        public int? GiftTotal1 { get; set; }
        public int? GiftCustomerType2 { get; set; }
        public Guid? GiftTypeId2 { get; set; }
        public int? GiftTotal2 { get; set; }
        public Guid CustomerId { get; set; }

        public override SendQuickGiftParameter ToParameter()
        {
            return new SendQuickGiftParameter
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
