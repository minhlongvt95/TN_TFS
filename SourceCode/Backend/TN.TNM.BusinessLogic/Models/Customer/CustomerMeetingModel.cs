using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.BusinessLogic.Models.Customer
{
    public class CustomerMeetingModel : BaseModel<CustomerMeetingEntityModel>
    {
        public Guid? CustomerMeetingId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid EmployeeId { get; set; }
        public string Title { get; set; }
        public string LocationMeeting { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? StartHours { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? EndHours { get; set; }
        public string Content { get; set; }
        public string Participants { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public string CreateByName { get; set; }
        public bool IsShowLink { get; set; }
        public CustomerMeetingModel()
        {

        }

        public CustomerMeetingModel(CustomerMeetingEntityModel model)
        {
            Mapper(model, this);
        }

        public override CustomerMeetingEntityModel ToEntity()
        {
            var entity = new CustomerMeetingEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
