using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class UpdateQuoteRequest : BaseRequest<UpdateQuoteParameter>
    {
        public QuoteModel Quote { get; set; }
        public List<QuoteDetailModel> QuoteDetail { get; set; }
        public int TypeAccount { get; set; }
        public List<QuoteDocument> FileList { get; set; }
        public override UpdateQuoteParameter ToParameter()
        {
            var QuoteProductDetailProductAttributeValue = new QuoteProductDetailProductAttributeValue();
            List<QuoteDetail> ListQuoteDetail = new List<QuoteDetail>();
            QuoteDetail.ForEach(item =>
            {
                var quoteDetailObject = new QuoteDetail();
                quoteDetailObject = item.ToEntity();
                List<QuoteProductDetailProductAttributeValue> QuoteProductDetailProductAttributeValueList = new List<QuoteProductDetailProductAttributeValue>();
                if (item.QuoteProductDetailProductAttributeValue != null)
                {
                    item.QuoteProductDetailProductAttributeValue.ForEach(itemX =>
                    {
                        QuoteProductDetailProductAttributeValueList.Add(itemX.ToEntity());
                    });
                    quoteDetailObject.QuoteProductDetailProductAttributeValue = QuoteProductDetailProductAttributeValueList;
                }
                ListQuoteDetail.Add(quoteDetailObject);
            });

            return new UpdateQuoteParameter
            {
                Quote = Quote.ToEntity(),
                QuoteDetail = ListQuoteDetail,
                TypeAccount = TypeAccount,
                FileList = FileList,
                UserId = this.UserId
            };
        }
    }
}
