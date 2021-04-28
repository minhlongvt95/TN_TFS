using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Lead
{
    public class LeadDashBoardEntityModel
    {
        public List<LeadReportByMonthModel> ListInvestFundReport { get; set; } // nguồn tiềm năng
        public List<LeadReportByMonthModel> ListPotentialReport { get; set; } // mức độ tiềm năng
        public List<LeadReportByMonthModel> ListInterestedGroupReport { get; set; } // nhu cầu
        public List<LeadSumaryByMonth> ListLeadSumaryByMonth { get; set; }
        public List<LeadReportModel> TopNewestLead { get; set; } // mới tạo 
        public List<LeadReportModel> TopApprovalLead { get; set; } // đã xác nhận (chờ làm báo giá + chờ làm hồ sơ thầu)

        public LeadDashBoardEntityModel()
        {
            this.ListInvestFundReport = new List<LeadReportByMonthModel>();
            this.ListPotentialReport = new List<LeadReportByMonthModel>();
            this.ListInterestedGroupReport = new List<LeadReportByMonthModel>();
            this.TopNewestLead = new List<LeadReportModel>();
            this.TopApprovalLead = new List<LeadReportModel>();
            this.ListLeadSumaryByMonth = new List<LeadSumaryByMonth>();
        }
    }

    public class LeadReportByMonthModel
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int Month { get; set; }
        public decimal PercentValue { get; set; }
        public decimal Value { get; set; }
    }

    public class LeadReportModel
    {
        public Guid LeadId { get; set; }
        public string LeadName { get; set; }
        public Guid? ContactId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Guid? PersonInChangeId { get; set; }
        public string PersonInChangeName { get; set; }
        public Guid StatusId { get; set; }
        public string StatusName { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
    }

    public class LeadSumaryByMonth
    {
        public int Month { get; set; }
        public decimal TotalInvestFundByMonth { get; set; }
        public decimal TotalPotentialByMonth { get; set; }
        public decimal TotalInterestedByMonth { get; set; }

        public LeadSumaryByMonth()
        {
            this.TotalInvestFundByMonth = 0;
            this.TotalPotentialByMonth = 0;
            this.TotalInterestedByMonth = 0;
        }
    }
}
