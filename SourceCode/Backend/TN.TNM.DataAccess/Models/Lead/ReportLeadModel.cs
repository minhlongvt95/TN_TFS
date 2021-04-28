using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Lead
{
    public class ReportLeadModel
    {
        public string LeadName { get; set; }
        public string PotentialSource { get; set; }
        public string Provincial { get; set; }
        public string PicCode { get; set; }
        public string PicName { get; set; }
        public string Month { get; set; }
        public double? DayCount { get; set; }
        public int WinCount { get; set; }
        public int LoseCount { get; set; }
        public int UndefinedCount { get; set; }
        public string ProbabilityName { get; set; }
        public string StatusName { get; set; }
        public decimal SumAmount { get; set; }

        public Guid LeadId { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? PotentialSourceId { get; set; }
        public Guid? ProbabilityId { get; set; }
        public Guid? PersonInChargeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? MonthTime { get; set; }
    }
}
