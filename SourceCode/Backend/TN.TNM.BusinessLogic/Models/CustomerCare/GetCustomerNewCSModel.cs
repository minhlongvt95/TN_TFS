using System;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Models.CustomerCare
{
    public class GetCustomerNewCSModel : BaseModel<GetCustomerNewCSEntityModel>
    {
        public Guid ContactId { get; set; }
        public Guid ObjectId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDay { get; set; }
        public Guid? EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string AvataUrl { get; set; }        

        public GetCustomerNewCSModel() { }

        public GetCustomerNewCSModel(GetCustomerNewCSEntityModel entity) : base(entity)
        {
            Mapper(entity, this);
        }
        
        public override GetCustomerNewCSEntityModel ToEntity()
        {
            var entity = new GetCustomerNewCSEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
