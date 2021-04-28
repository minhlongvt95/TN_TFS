using System;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.BusinessLogic.Models.CustomerCare
{
    public class FilterCustomerModel : BaseModel<DataAccess.Databases.Entities.Customer>
    {
        public Guid CustomerId { get; set; }
        public Guid ContactId { get; set; }
        public string CustomerName { get; set; }
        public string PicName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public FilterCustomerModel() { }

        public FilterCustomerModel(DataAccess.Databases.Entities.Customer entity) : base(entity)
        {

        }

        public FilterCustomerModel(CustomerEntityModel model)
        {
            Mapper(model, this);
        }

        public override DataAccess.Databases.Entities.Customer ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.Customer();
            Mapper(this, entity);
            return entity;
        }
    }
}
