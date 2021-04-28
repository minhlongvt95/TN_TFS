using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Models.Customer
{
    public class CustomerCareInforBusinessModel
    {
        public string EmployeeName { get; set; }
        public string EmployeePosition { get; set; }
        public Guid EmployeeCharge { get; set; }
        public List<CustomerCareForWeekBusinessModel> Week1 { get; set; }
        public List<CustomerCareForWeekBusinessModel> Week2 { get; set; }
        public List<CustomerCareForWeekBusinessModel> Week3 { get; set; }
        public List<CustomerCareForWeekBusinessModel> Week4 { get; set; }
        public List<CustomerCareForWeekBusinessModel> Week5 { get; set; }
    }
}
