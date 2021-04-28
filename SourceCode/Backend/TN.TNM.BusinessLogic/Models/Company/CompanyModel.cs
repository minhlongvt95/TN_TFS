using System;

namespace TN.TNM.BusinessLogic.Models.Company
{
    public class CompanyModel : BaseModel<DataAccess.Databases.Entities.Company>
    {
        public CompanyModel() { }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public CompanyModel(DataAccess.Databases.Entities.Company entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len      
        }

        public override DataAccess.Databases.Entities.Company ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.Company();
            Mapper(this, entity);
            return entity;
        }
    }
}
