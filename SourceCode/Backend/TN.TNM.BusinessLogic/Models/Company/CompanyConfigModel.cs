using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.BusinessLogic.Models.Company
{
    public class CompanyConfigModel : BaseModel<CompanyConfiguration>
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string TaxCode { get; set; }
        public Guid? BankAccountId { get; set; }
        public string CompanyAddress { get; set; }
        public string ContactName { get; set; }
        public string ContactRole { get; set; }
        public CompanyConfigModel() { }
        public CompanyConfigModel(CompanyConfigEntityModel entity)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(entity, this);
        }
        public CompanyConfigModel(CompanyConfiguration entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override CompanyConfiguration ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new CompanyConfiguration();
            Mapper(this, entity);
            return entity;
        }
    }
}
