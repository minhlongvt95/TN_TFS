using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Models.Quote
{
    public class AdditionalInformationModel : BaseModel<DataAccess.Databases.Entities.AdditionalInformation>
    {
        public Guid AdditionalInformationId { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? Ordinal { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
        public int? OrderNumber { get; set; }

        public AdditionalInformationModel() { }

        public AdditionalInformationModel(DataAccess.Databases.Entities.AdditionalInformation entity) : base(entity)
        {
            // Mapper(entity, this);
        }
        public AdditionalInformationModel(AdditionalInformationEntityModel model)
        {
            Mapper(model, this);
        }
        public override DataAccess.Databases.Entities.AdditionalInformation ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.AdditionalInformation();
            Mapper(this, entity);
            return entity;
        }
    }
}
