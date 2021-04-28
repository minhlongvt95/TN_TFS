using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CaseActivities
    {
        public Guid CaseActivitiesId { get; set; }
        public Guid? CaseActivitiesType { get; set; }
        public string CaseActivitiesContent { get; set; }
        public Guid? CaseId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
        public Guid? TenantId { get; set; }

        public Case Case { get; set; }
    }
}
