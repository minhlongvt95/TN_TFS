using System;
using TN.TNM.DataAccess.Models.Receivable;

namespace TN.TNM.BusinessLogic.Models.Receivable
{
    public class SalesReportModel : BaseModel<SalesReportEntityModel>
    {
        public string SalesReportMonth { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Benefits { get; set; }
        public SalesReportModel() { }
        public SalesReportModel(SalesReportEntityModel entity)
        {
            Mapper(entity, this);
        }
        public SalesReportEntityModel ToModel()
        {
            //Code tien xu ly model truoc khi day vao DB
            var model = new SalesReportEntityModel();
            Mapper(this, model);
            return model;
        }

        public override SalesReportEntityModel ToEntity()
        {
            throw new NotImplementedException();
        }
    }
}
