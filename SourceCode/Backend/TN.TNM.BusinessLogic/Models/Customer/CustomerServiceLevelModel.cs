using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.BusinessLogic.Models.Customer
{
    public class CustomerServiceLevelModel : BaseModel<CustomerServiceLevel>
    {
        public Guid CustomerServiceLevelId { get; set; }
        public string CustomerServiceLevelName { get; set; }
        public string CustomerServiceLevelCode { get; set; }
        public decimal? MinimumValue { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }

        public CustomerServiceLevelModel() { }

        public CustomerServiceLevelModel(CustomerServiceLevel entity) : base(entity)
        {
            Mapper(entity, this);
        }


        public override CustomerServiceLevel ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new CustomerServiceLevel();
            Mapper(this, entity);
            return entity;
        }
    }
}
