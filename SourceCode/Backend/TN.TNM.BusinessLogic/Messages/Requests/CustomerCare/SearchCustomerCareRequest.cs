using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class SearchCustomerCareRequest : BaseRequest<SearchCustomerCareParameter>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string CustomerCareTitle { get; set; }
        public string CustomerCareCode { get; set; }
        public List<Guid> ListTypeCusCareId { get; set; }
        public List<Guid> PicName { get; set; }
        public List<Guid> Status { get; set; }
        public string CustomerCareContent { get; set; }
        public List<int> ProgramType { get; set; }
        

        public override SearchCustomerCareParameter ToParameter()
        {
            return new SearchCustomerCareParameter
            {
                FromDate = this.FromDate,
                ToDate = this.ToDate,
                CustomerCareTitle = this.CustomerCareTitle,
                CustomerCareContent = this.CustomerCareContent,
                CustomerCareCode = this.CustomerCareCode,
                ListTypeCusCareId = this.ListTypeCusCareId,
                PicName = this.PicName,
                Status = this.Status,
                ProgramType = this.ProgramType
            };
        }
    }
}
