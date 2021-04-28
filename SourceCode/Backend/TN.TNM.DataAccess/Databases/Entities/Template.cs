using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Template
    {
        public Guid TemplateId { get; set; }
        public string TemplateTitle { get; set; }
        public string TemplateContent { get; set; }
        public int? Type { get; set; }
        public int? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
        public Guid? TenantId { get; set; }
    }
}
