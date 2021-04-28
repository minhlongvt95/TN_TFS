using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.BusinessLogic.Models.Employee
{
    public class PositionModel : BaseModel<Position>
    {
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

        public PositionModel(Position entity) : base(entity)
        {

        }

        public override Position ToEntity()
        {
            var entity = new Position();
            Mapper(this, entity);
            return entity;
        }
    }
}
