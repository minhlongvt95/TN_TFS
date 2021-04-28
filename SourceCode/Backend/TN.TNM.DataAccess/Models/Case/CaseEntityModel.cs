using System;

namespace TN.TNM.DataAccess.Models.Case
{
    public class CaseEntityModel
    {
        public Guid CaseId { get; set; }
        public Guid? ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string CasteTitle { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? AsignTo { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
    }
}
