using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Models.CustomerCare
{
    public class CustomerCareFeedBackModel : BaseModel<CustomerCareFeedBack>
    {
        public Guid CustomerCareFeedBackId { get; set; }
        public DateTime? FeedBackFromDate { get; set; }
        public DateTime? FeedBackToDate { get; set; }
        public Guid? FeedbackType { get; set; }
        public Guid? FeedBackCode { get; set; }
        public string FeedBackContent { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? CustomerCareId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }

        public CustomerCareFeedBackModel() { }

        public CustomerCareFeedBackModel(CustomerCareFeedBackEntityModel CustomerCareFeedBackEntityModel) {
            Mapper(CustomerCareFeedBackEntityModel, this);
        }

        public CustomerCareFeedBackModel(CustomerCareFeedBack entity) : base(entity)
        {
            Mapper(entity, this);
        }

       
        public override CustomerCareFeedBack ToEntity()
        {
            var entity = new CustomerCareFeedBack();
            Mapper(this, entity);
            return entity;
        }
    }
}
