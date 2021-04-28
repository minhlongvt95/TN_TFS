using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetListQuestionAnswerBySearchResponse : BaseResponse
    {
        public List<dynamic> CustomerAdditionalInformationList { get; set; }
    }
}
