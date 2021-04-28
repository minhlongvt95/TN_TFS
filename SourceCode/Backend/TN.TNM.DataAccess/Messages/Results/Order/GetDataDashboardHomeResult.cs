using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Lead;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetDataDashboardHomeResult : BaseResult
    {
        public decimal TotalSalesOfWeek { get; set; }
        public decimal TotalSalesOfMonth { get; set; }
        public decimal TotalSalesOfQuarter { get; set; }
        public decimal TotalSalesOfWeekPress { get; set; }
        public decimal TotalSalesOfMonthPress { get; set; }
        public decimal TotalSalesOfQuarterPress { get; set; }
        public List<QuoteEntityModel> ListQuote { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
        public List<CustomerOrderEntityModel> ListOrderNew { get; set; }
        public List<CustomerMeetingEntityModel> ListCustomerMeeting { get; set; }
        public List<LeadMeetingEntityModel> ListLeadMeeting { get; set; }
        public List<CustomerEntityModel> ListCusBirthdayOfWeek { get; set; }
        public List<EmployeeEntityModel> ListEmployeeBirthDayOfWeek { get; set; }
        public List<EmployeeEntityModel> ListParticipants { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
    }
}
