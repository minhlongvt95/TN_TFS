using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class ReportLeadRequest : BaseRequest<ReportLeadParameter>
    {
        public string ReportCode { get; set; }
        public List<Guid> ListEmployeeId { get; set; }
        public List<Guid> ListSourceId { get; set; }
        public List<Guid> ListGeographicalAreaId { get; set; }
        public TimeSearchModel TimeParameter { get; set; }
        public override ReportLeadParameter ToParameter()
        {
            return new ReportLeadParameter
            {
                UserId = this.UserId,
                ReportCode = this.ReportCode,
                ListEmployeeId = this.ListEmployeeId,
                ListSourceId = this.ListSourceId,
                ListGeographicalAreaId = this.ListGeographicalAreaId,
                TimeParameter = this.TimeParameter
            };
        }
    }
}
