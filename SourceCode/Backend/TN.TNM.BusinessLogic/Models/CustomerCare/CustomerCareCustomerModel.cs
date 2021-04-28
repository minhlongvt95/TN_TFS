using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.BusinessLogic.Models.CustomerCare
{
    public class CustomerCareCustomerModel : BaseModel<CustomerCareCustomer>
    {
        public Guid CustomerCareCustomerId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? CustomerCareId { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }

        public CustomerCareCustomerModel() { }

        public CustomerCareCustomerModel(CustomerCareCustomer entity) : base(entity)
        {
            Mapper(entity, this);
        }


        public override CustomerCareCustomer ToEntity()
        {
            var entity = new CustomerCareCustomer();
            Mapper(this, entity);
            return entity;
        }
    }
}
