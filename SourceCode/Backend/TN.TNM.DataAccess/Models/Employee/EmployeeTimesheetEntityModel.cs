using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class EmployeeTimesheetEntityModel
    {
        public Guid EmployeeTimesheetId { get; set; }
        public Guid? EmployeeId { get; set; }
        public DateTime? TimesheetDate { get; set; }
        public TimeSpan? CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }
        public decimal? ActualWorkingDay { get; set; }
        public decimal? Overtime { get; set; }
        public decimal? ReductionAmount { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }

        public EmployeeTimesheetEntityModel(EmployeeTimesheet entity)
        {
            EmployeeTimesheetId = entity.EmployeeTimesheetId;
            EmployeeId = entity.EmployeeId;
            TimesheetDate = entity.TimesheetDate;
            CheckIn = entity.CheckIn;
            CheckOut = entity.CheckOut;
            ActualWorkingDay = entity.ActualWorkingDay;
            Overtime = entity.Overtime;
            ReductionAmount = entity.ReductionAmount;
            CreateById = entity.CreateById;
            CreateDate = entity.CreateDate;
            UpdateById = entity.UpdateById;
            UpdateDate = entity.UpdateDate;
        }
    }
}
