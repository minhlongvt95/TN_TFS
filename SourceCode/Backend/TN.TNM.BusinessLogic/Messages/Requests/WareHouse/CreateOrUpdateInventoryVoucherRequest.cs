using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.WareHouse;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class CreateOrUpdateInventoryVoucherRequest : BaseRequest<CreateOrUpdateInventoryVoucherParameter>
    {
        public string inventoryReceivingVoucher { get; set; }
        public string  inventoryReceivingVoucherMapping { get; set; }
        public List<IFormFile> fileList { get; set; }
        public string noteContent { get; set; }

        public override CreateOrUpdateInventoryVoucherParameter ToParameter()
        {
            var ConvertToInventoryReceivingVoucherModel = JsonConvert.DeserializeObject<BusinessLogic.Models.WareHouse.InventoryReceivingVoucherModel>(inventoryReceivingVoucher);
            var ConvertToListinventoryReceivingVoucherMapping = JsonConvert.DeserializeObject<List<GetVendorOrderDetailByVenderOrderIdModel>>(inventoryReceivingVoucherMapping);
            var lst = new List<GetVendorOrderDetailByVenderOrderIdEntityModel>();
            ConvertToListinventoryReceivingVoucherMapping.ForEach(step =>
            {
                var obj = new GetVendorOrderDetailByVenderOrderIdEntityModel()
                {
                    InventoryReceivingVoucherMappingId=step.InventoryReceivingVoucherMappingId,
                    VendorOrderId = step.VendorOrderId,
                    VendorOrderDetailId=step.VendorOrderDetailId,
                    VendorOrderCode = step.VendorOrderCode,
                    ProductId = step.ProductId,
                    ProductCode = step.ProductCode,
                    ProductName = step.ProductName,
                    UnitId = step.UnitId,
                    UnitName = step.UnitName,
                    QuantityRequire = step.QuantityRequire,
                    Quantity = step.Quantity,
                    Price = step.Price,
                    WareHouseId = step.WareHouseId,
                    WareHouseName = step.WareHouseName,
                    Note = step.Note,
                    TotalSerial = step.TotalSerial,
                    ListSerial = step.ListSerial,
                    CurrencyUnit=step.CurrencyUnit,
                    ExchangeRate=step.ExchangeRate,
                    Vat=step.Vat,
                    DiscountValue=step.Vat,
                    DiscountType=step.DiscountType,
                    SumAmount=step.SumAmount,
                    NameMoneyUnit=step.NameMoneyUnit
                };
                lst.Add(obj);
            });

            return new CreateOrUpdateInventoryVoucherParameter()
            {
                UserId = UserId,
                inventoryReceivingVoucher = ConvertToInventoryReceivingVoucherModel.ToEntity(),
                inventoryReceivingVoucherMapping = lst,
                fileList=this.fileList,
                noteContent=this.noteContent
            };
        }
    }
}

