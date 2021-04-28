using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class ReportLeadResult : BaseResult
    {
        public List<ReportLeadModel> ListReportLeadFollowAge { get; set; }
        public List<ReportLeadModel> ListReportLeadFollowPic { get; set; }
        public List<ReportLeadModel> ListReportLeadFollowSource { get; set; }
        public List<ReportLeadModel> ListReportLeadFollowProvincial { get; set; }
        public List<ReportLeadModel> ListReportLeadFollowMonth { get; set; }
    }
}
