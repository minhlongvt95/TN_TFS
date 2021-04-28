using System;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Models.CustomerCare
{
    public class GetCustomerBirthDayModel : BaseModel<GetCustomerBirthDayEntityModel>
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

        public GetCustomerBirthDayModel() { }

        public GetCustomerBirthDayModel(GetCustomerBirthDayEntityModel entity) : base(entity)
        {
            Mapper(entity, this);
        }


        public override GetCustomerBirthDayEntityModel ToEntity()
        {
            var entity = new GetCustomerBirthDayEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
