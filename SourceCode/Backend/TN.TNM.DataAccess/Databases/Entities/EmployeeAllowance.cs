using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class EmployeeAllowance
    {
        public Guid EmployeeAllowanceId { get; set; }
        public decimal? LunchAllowance { get; set; }
        public decimal? MaternityAllowance { get; set; }
        public decimal? FuelAllowance { get; set; }
        public decimal? PhoneAllowance { get; set; }
        public decimal? OtherAllownce { get; set; }
        public Guid? EmployeeId { get; set; }
        public bool? FreeTimeUnlimited { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
        public Guid? TenantId { get; set; }

        public Employee Employee { get; set; }
    }
}
