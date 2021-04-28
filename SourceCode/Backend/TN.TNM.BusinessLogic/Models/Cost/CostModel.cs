using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Cost;

namespace TN.TNM.BusinessLogic.Models.Cost
{
    public class CostModel : BaseModel<DataAccess.Databases.Entities.Cost>
    {
        public Guid CostId { get; set; }
        public string CostCode { get; set; }
        public string CostName { get; set; }
        public string CostCodeName { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? StatusId { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string StatusName { get; set; }
        public string OrganizationName { get; set; }

        public CostModel() { }

        public CostModel(CostEntityModel entity)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(entity, this);
        }

        public CostModel(DataAccess.Databases.Entities.Cost entity) : base(entity)
        {
            //Xử lý sau khi lấy từ DB lên
        }
        
        public override DataAccess.Databases.Entities.Cost ToEntity()
        {
            //Code tiền xử lý model trước khí đẩy vào DB
            var entity = new DataAccess.Databases.Entities.Cost();
            Mapper(this, entity);
            return entity;
        }
    }
}
