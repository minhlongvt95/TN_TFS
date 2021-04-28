using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.BusinessLogic.Models.Email
{
    public class EmailTemplateModel: BaseModel<DataAccess.Databases.Entities.EmailTemplate>
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

        public EmailTemplateModel() { }

        public EmailTemplateModel(DataAccess.Databases.Entities.EmailTemplate entity)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(entity, this);
        }

        public override DataAccess.Databases.Entities.EmailTemplate ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.EmailTemplate();
            Mapper(this, entity);
            return entity;
        }
    }
}
