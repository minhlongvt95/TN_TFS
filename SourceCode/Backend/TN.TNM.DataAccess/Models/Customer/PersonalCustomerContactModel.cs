using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Customer
{
    public class PersonalCustomerContactModel
    {
        public Guid ContactId { get; set; }
        public string Email { get; set; }
        public string WorkEmail { get; set; }
        public string OtherEmail { get; set; }
        public string Phone { get; set; }
        public string WorkPhone { get; set; }
        public string OtherPhone { get; set; }
        public Guid? AreaId { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? WardId { get; set; }
        public string Address { get; set; }
        public string Other { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
