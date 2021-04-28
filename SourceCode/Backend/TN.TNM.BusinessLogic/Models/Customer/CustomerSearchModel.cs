using System;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.BusinessLogic.Models.Customer
{
    public class CustomerSearchModel : BaseModel<DataAccess.Databases.Entities.Customer>
    {
        public Guid CustomerId { get; set; }
        public Guid ContactId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string PicName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public int CountCustomerInfo { get; set; }
        //public string CusAvatarUrl { get; set; }
        //public string PicAvatarUrl { get; set; }
        public string StatusName { get; set; }
        public string BackgroupStatus { get; set; }
        public string StatusCareName { get; set; }
        public string BackgroundStatusCare { get; set; }
        public string ColorStatusCare { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime CreatedDate { get; set; }

        public CustomerSearchModel() { }

        public CustomerSearchModel(DataAccess.Databases.Entities.Customer entity) : base(entity)
        {

        }

        public CustomerSearchModel(CustomerEntityModel model)
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
