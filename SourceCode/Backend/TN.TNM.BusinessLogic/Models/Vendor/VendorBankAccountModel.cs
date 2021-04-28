using System;
using System.Collections.Generic;
using System.Text;
using Entities = N8.ISUZU.DataAccess.Databases.Entities;

namespace N8.ISUZU.BusinessLogic.Models.Vendor
{
    public class VendorBankAccountModel : BaseModel<Entities.VendorBankAccount>
    {
        public Guid? VendorBankAccountId { get; set; }
        public Guid? VendorId { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string AccountName { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }

        public VendorBankAccountModel() { }

        public VendorBankAccountModel(Entities.VendorBankAccount entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override Entities.VendorBankAccount ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new Entities.VendorBankAccount();
            Mapper(this, entity);
            return entity;
        }
    }
}
