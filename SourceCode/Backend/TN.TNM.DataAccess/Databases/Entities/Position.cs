using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Position
    {
        public Position()
        {
            InverseParent = new HashSet<Position>();
        }

        public Guid PositionId { get; set; }
        public string PositionName { get; set; }
        public string PositionCode { get; set; }
        public Guid? ParentId { get; set; }
        public int Level { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public Guid? TenantId { get; set; }

        public Position Parent { get; set; }
        public ICollection<Position> InverseParent { get; set; }
    }
}
