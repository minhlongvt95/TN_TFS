using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Lead;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetDataDashboardHomeResponse : BaseResponse
    {
        public decimal TotalSalesOfWeek { get; set; }
        public decimal TotalSalesOfMonth { get; set; }
        public decimal TotalSalesOfQuarter { get; set; }
        public decimal TotalSalesOfWeekPress { get; set; }
        public decimal TotalSalesOfMonthPress { get; set; }
        public decimal TotalSalesOfQuarterPress { get; set; }
        public List<QuoteModel> ListQuote { get; set; }
        public List<CustomerModel> ListCustomer { get; set; }
        public List<CustomerOrderModel> ListOrderNew { get; set; }
        public List<CustomerMeetingModel> ListCustomerMeeting { get; set; }
        public List<LeadMeetingModel> ListLeadMeeting { get; set; }
        public List<CustomerModel> ListCusBirthdayOfWeek { get; set; }
        public List<EmployeeModel> ListEmployeeBirthDayOfWeek { get; set; }
        public List<EmployeeEntityModel> ListParticipants { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
    }
}
