using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Email
{
    public class SendEmailRequest: BaseRequest<SendEmailParameter>
    {
        public int SendType { get; set; }
        public DataAccess.Models.Email.SendEmailEntityModel SendEmailEntityModel { get; set; }

        public override SendEmailParameter ToParameter()
        {
            var newSendEmailEntityModel = new DataAccess.Models.Email.SendEmailEntityModel();
            newSendEmailEntityModel = SendEmailEntityModel;

            var listQuoteDetailToSendEmail = new List<DataAccess.Models.Email.QuoteDetailToSendEmailModel>();

            SendEmailEntityModel.ListQuoteDetailToSendEmail?.ForEach(quoteDetail =>
            {
                listQuoteDetailToSendEmail.Add(new DataAccess.Models.Email.QuoteDetailToSendEmailModel
                {
                    DiscountValue = quoteDetail.DiscountValue,
                    ProductName = quoteDetail.ProductName,
                    ProductNameUnit = quoteDetail.ProductNameUnit,
                    Quantity = quoteDetail.Quantity,
                    SumAmount = quoteDetail.SumAmount,
                    UnitPrice = quoteDetail.UnitPrice,
                    Vat = quoteDetail.Vat
                });
            });

            newSendEmailEntityModel.ListQuoteDetailToSendEmail = listQuoteDetailToSendEmail;

            return new SendEmailParameter()
            {
                SendType = SendType,
                SendEmailEntityModel = newSendEmailEntityModel,
                UserId = UserId
            };
        }
    }
}
