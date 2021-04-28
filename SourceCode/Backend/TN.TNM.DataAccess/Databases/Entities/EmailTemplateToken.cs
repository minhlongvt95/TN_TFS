using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class EmailTemplateToken
    {
        public Guid EmailTemplateTokenId { get; set; }
        public string TokenLabel { get; set; }
        public string TokenCode { get; set; }
        public Guid EmailTemplateTypeId { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
