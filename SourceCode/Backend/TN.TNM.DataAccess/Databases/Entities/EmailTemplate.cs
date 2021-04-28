using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class EmailTemplate
    {
        public Guid EmailTemplateId { get; set; }
        public string EmailTemplateName { get; set; }
        public string EmailTemplateTitle { get; set; }
        public string EmailTemplateContent { get; set; }
        public Guid EmailTemplateTypeId { get; set; }
        public Guid EmailTemplateStatusId { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public bool? IsAutomatic { get; set; }
    }
}
