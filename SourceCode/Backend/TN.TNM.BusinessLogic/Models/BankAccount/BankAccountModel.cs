using System;
using TN.TNM.DataAccess.Models.BankAccount;

namespace TN.TNM.BusinessLogic.Models.BankAccount
{
    public class BankAccountModel : BaseModel<DataAccess.Databases.Entities.BankAccount>
    {
        public Guid? BankAccountId { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string BankDetail { get; set; }
        public string AccountName { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }

        public BankAccountModel(){}
        public BankAccountModel(DataAccess.Databases.Entities.BankAccount entity) : base(entity)
        {
            Mapper(entity, this);
        }
        public BankAccountModel(BankAccountEntityModel model)
        {
            //Mapper(this, model);
            Mapper(model,this );
        }
        public override DataAccess.Databases.Entities.BankAccount ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.BankAccount();
            Mapper(this, entity);
            return entity;
        }
    }
}
