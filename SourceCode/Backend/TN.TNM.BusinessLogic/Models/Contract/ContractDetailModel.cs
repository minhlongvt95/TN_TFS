using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Contract;

namespace TN.TNM.BusinessLogic.Models.Contract
{
    public class ContractDetailModel : BaseModel<DataAccess.Databases.Entities.ContractDetail>
    {
        public Guid ContractDetailId { get; set; }
        public Guid ContractId { get; set; }
        public Guid? VendorId { get; set; }
        public Guid? ProductId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? QuantityOdered { get; set; }
        public decimal? UnitPrice { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Tax { get; set; }
        public decimal? Vat { get; set; }
        public decimal? GuaranteeTime { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public string Description { get; set; }
        public short? OrderDetailType { get; set; }
        public Guid? UnitId { get; set; }
        public string IncurredUnit { get; set; }
        public short? CostsQuoteType { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string NameVendor { get; set; }
        public string NameProduct { get; set; }
        public string NameProductUnit { get; set; }
        public string NameMoneyUnit { get; set; }
        public decimal SumAmount { get; set; }
        public decimal? QuantityOrderProduct { get; set; }
        public string ProductName { get; set; }
        public int? OrderNumber { get; set; }

        public List<ContractDetailProductAttributeModel> ContractProductDetailProductAttributeValue { get; set; }
        public ContractDetailModel() { }

        public ContractDetailModel(DataAccess.Databases.Entities.ContractDetail entity) : base(entity)
        {

        }

        public ContractDetailModel(ContractDetailEntityModel model)
        {
            Mapper(model, this);
        }

        public override DataAccess.Databases.Entities.ContractDetail ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.ContractDetail();
            Mapper(this, entity);
            return entity;
        }
        public ContractDetailEntityModel ToEntityModel()
        {
            var entity = new ContractDetailEntityModel();
            Mapper(this, entity);
            return entity;
        }

    }
}
