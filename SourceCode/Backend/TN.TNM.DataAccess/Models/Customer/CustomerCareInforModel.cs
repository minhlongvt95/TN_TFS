using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Customer
{
    public class CustomerCareInforModel
    {
        public string EmployeeName { get; set; }
        public string EmployeePosition { get; set; }
        public Guid EmployeeCharge { get; set; }
        public List<CustomerCareForWeekModel> Week1 { get; set; }
        public List<CustomerCareForWeekModel> Week2 { get; set; }
        public List<CustomerCareForWeekModel> Week3 { get; set; }
        public List<CustomerCareForWeekModel> Week4 { get; set; }
        public List<CustomerCareForWeekModel> Week5 { get; set; }
    }
}
