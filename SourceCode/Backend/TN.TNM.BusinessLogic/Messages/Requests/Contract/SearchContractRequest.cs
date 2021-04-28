using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Contract;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contract
{
    public class SearchContractRequest : BaseRequest<SearchContractParameter>
    {
        public string ContractCode { get; set; }
        public string QuoteCode { get; set; }
        public List<Guid> ListProductId { get; set; }
        public List<Guid> ListEmployeeId { get; set; }
        public List<Guid> ListCutomerId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public override SearchContractParameter ToParameter()
        {
            return new SearchContractParameter
            {
                ContractCode = ContractCode,
                QuoteCode = QuoteCode,
                ListProductId = ListProductId,
                ListEmployeeId = ListEmployeeId,
                ListCutomerId = ListCutomerId,
                FromDate = FromDate,
                ToDate = ToDate,
                UserId = UserId
            };
        }
    }
}
