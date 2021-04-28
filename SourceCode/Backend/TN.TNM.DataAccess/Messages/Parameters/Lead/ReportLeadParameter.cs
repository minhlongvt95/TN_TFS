using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class ReportLeadParameter : BaseParameter
    {
        public string ReportCode { get; set; }
        public List<Guid> ListEmployeeId { get; set; }
        public List<Guid> ListSourceId { get; set; }
        public List<Guid> ListGeographicalAreaId { get; set; }
        public TimeSearchModel TimeParameter { get; set; }
    }
}
