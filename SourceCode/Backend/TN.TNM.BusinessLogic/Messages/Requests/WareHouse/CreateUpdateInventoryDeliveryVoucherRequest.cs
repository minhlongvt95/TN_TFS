using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.WareHouse;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class CreateUpdateInventoryDeliveryVoucherRequest : BaseRequest<CreateUpdateInventoryDeliveryVoucherParameter>
    {
        public string inventoryDeliveryVoucher { get; set; }
        public string inventoryyDeliveryVoucherMapping { get; set; }
        public List<IFormFile> fileList { get; set; }
        public string noteContent { get; set; }

        public override CreateUpdateInventoryDeliveryVoucherParameter ToParameter()
        {
            var ConvertToInventoryDeliveryVoucherModel = JsonConvert.DeserializeObject<InventoryDeliveryVoucherModel>(inventoryDeliveryVoucher);
            var ConvertToListInventoryDeliveryVoucherMapping = JsonConvert.DeserializeObject<List<InventoryDeliveryVoucherMappingModel>>(inventoryyDeliveryVoucherMapping);
            var lst = new List<InventoryDeliveryVoucherMappingEntityModel>();

            ConvertToListInventoryDeliveryVoucherMapping.ForEach(step =>
            {
                var obj = new InventoryDeliveryVoucherMappingEntityModel()
                {
                    InventoryDeliveryVoucherMappingId = step.InventoryDeliveryVoucherMappingId,
                    ProductId = step.ProductId,
                    ProductCode = step.ProductCode,
                    ProductName = step.ProductName,
                    UnitId = step.UnitId,
                    UnitName = step.UnitName,
                    QuantityRequire = step.QuantityRequire,
                    Quantity = step.Quantity,
                    QuantityInventory=step.QuantityInventory,
                    Price = step.Price,
                    WarehouseId = step.WarehouseId,
                    WareHouseName = step.WareHouseName,
                    Note = step.Note,
                    TotalSerial = step.TotalSerial,
                    ListSerial = step.ListSerial,
                    CurrencyUnit = step.CurrencyUnit,
                    ExchangeRate = step.ExchangeRate,
                    Vat = step.Vat,
                    DiscountValue = step.Vat,
                    DiscountType = step.DiscountType,
                    SumAmount = step.SumAmount,
                    NameMoneyUnit = step.NameMoneyUnit
                };
                lst.Add(obj);
            });

            return new CreateUpdateInventoryDeliveryVoucherParameter
            {
                inventoryDeliveryVoucher= ConvertToInventoryDeliveryVoucherModel.ToEntity(),
                inventoryyDeliveryVoucherMapping=lst,
                fileList = this.fileList,
                noteContent = this.noteContent,
                UserId=this.UserId
            };
        }
    }
}
