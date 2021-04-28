using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Department
    {
        public Department()
        {
            Employee = new HashSet<Employee>();
            InverseParent = new HashSet<Department>();
        }

        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public bool? Active { get; set; }
        public Guid? ParentId { get; set; }

        public Department Parent { get; set; }
        public ICollection<Employee> Employee { get; set; }
        public ICollection<Department> InverseParent { get; set; }
    }
}
