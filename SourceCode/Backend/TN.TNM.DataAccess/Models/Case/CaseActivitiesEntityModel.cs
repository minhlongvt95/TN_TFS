using System;

namespace TN.TNM.DataAccess.Models.Case
{
    public class CaseActivitiesEntityModel
    {
        public Guid CaseActivitiesId { get; set; }
        public Guid? CaseActivitiesType { get; set; }
        public string CaseActivitiesContent { get; set; }
        public Guid? CaseId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
    }
}
