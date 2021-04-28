using System;

namespace TN.TNM.DataAccess.Models.CustomerCare
{
    public class GetCustomerBirthDayEntityModel
    {
        public Guid ContactId { get; set; }
        public Guid ObjectId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDay { get; set; }
        public Guid? EmployeeID{ get; set; }
        public string EmployeeName{ get; set; }
        public string AvataUrl{ get; set; }

    }
}
