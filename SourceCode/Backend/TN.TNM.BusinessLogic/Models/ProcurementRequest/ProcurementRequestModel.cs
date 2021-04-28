using System;
using TN.TNM.DataAccess.Models.ProcurementRequest;

namespace TN.TNM.BusinessLogic.Models.ProcurementRequest
{
    public class ProcurementRequestModel : BaseModel<DataAccess.Databases.Entities.ProcurementRequest>
    {
        public Guid ProcurementRequestId { get; set; }
        public string ProcurementCode { get; set; }
        public string ProcurementContent { get; set; }
        public string Description { get; set; }
        public Guid? RequestEmployeeId { get; set; }
        public string EmployeePhone { get; set; }
        public Guid? Unit { get; set; }
        public Guid? ApproverId { get; set; }
        public Guid? ApproverPostion { get; set; }
        public string ApproverPostionName { get; set; }
        public string Explain { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string RequestEmployeeName { get; set; }
        public string ApproverName { get; set; }
        public string StatusName { get; set; }
        public string StatusCode { get; set; }
        public string OrganizationName { get; set; }
        public decimal TotalMoney { get; set; }
        public string VendorName { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public Guid? OrderId { get; set; }
        public decimal SumQuantity { get; set; }
        public decimal SumQuantityApproval { get; set; }
        public decimal? SumQuantityPO { get; set; } // Số lượng đặt hàng
        public string UnitName { get; set; } // Đơn vị tính
        public int STT { get; set; } // Số thứ tự
        public string ProductCode { get; set; } // mã hàng
        public string ProductName { get; set; } // Tên hàng
        public Guid? ProductId { get; set; } // Id hàng
        public Guid? VendorId { get; set; } // Id ncc
        public Guid? BudgetId { get; set; } // Id dự toán

        public ProcurementRequestModel() { }
        public ProcurementRequestModel(DataAccess.Databases.Entities.ProcurementRequest entity) : base(entity)
        {
            
        }
        public ProcurementRequestModel(ProcurementRequestEntityModel model)
        {
            Mapper(model, this);
        }

        public override DataAccess.Databases.Entities.ProcurementRequest ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.ProcurementRequest();
            Mapper(this, entity);
            return entity;
        }
    }
}
