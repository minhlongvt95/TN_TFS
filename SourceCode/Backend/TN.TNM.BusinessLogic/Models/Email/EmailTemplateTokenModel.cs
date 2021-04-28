using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Models.Email
{
    public class EmailTemplateTokenModel: BaseModel<DataAccess.Databases.Entities.EmailTemplateToken>
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

        public EmailTemplateTokenModel() { }

        public EmailTemplateTokenModel(DataAccess.Databases.Entities.EmailTemplateToken entity)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(entity, this);
        }

        public override DataAccess.Databases.Entities.EmailTemplateToken ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.EmailTemplateToken();
            Mapper(this, entity);
            return entity;
        }
    }
}
