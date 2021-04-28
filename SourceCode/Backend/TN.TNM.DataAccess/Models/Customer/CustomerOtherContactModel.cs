using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Customer
{
    public class CustomerOtherContactModel
    {
        public Guid? ContactId { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactName { get; set; }
        public string Gender { get; set; }
        public string GenderName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }    //Chức vụ
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Other { get; set; }   //Thông tin khác
        public DateTime? CreatedDate { get; set; }
    }
}
