using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetListQuestionAnswerBySearchResult : BaseResult
    {
        public List<dynamic> CustomerAdditionalInformationList { get; set; }
    }
}
