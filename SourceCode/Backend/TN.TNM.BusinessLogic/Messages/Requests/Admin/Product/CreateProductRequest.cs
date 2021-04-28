using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.BusinessLogic.Models.ProductAttributeCategory;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;
using Microsoft.AspNetCore.Http;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Product
{
    public class CreateProductRequest: BaseRequest<CreateProductParameter>
    {
        public DataAccess.Models.Product.ProductEntityModel Product { get; set; }
        public List<DataAccess.Models.Product.ProductVendorMappingEntityModel> ListProductVendorMapping { get; set; }
        public List<DataAccess.Models.ProductAttributeCategory.ProductAttributeCategoryEntityModel> ListProductAttributeCategory { get; set; }
        public List<DataAccess.Models.Product.ProductQuantityInWarehouseEntityModel> ListInventoryReport { get; set; }
        public List<ProductImage> ListProductImage { get; set; }
        public List<DataAccess.Models.Product.ProductBillOfMaterialsEntityModel> ListProductBillOfMaterials { get; set; }

        public override CreateProductParameter ToParameter()
        {
            var listProductAttributeCategory = new List<DataAccess.Models.ProductAttributeCategory.ProductAttributeCategoryEntityModel>();
            ListProductAttributeCategory.ForEach(attr =>
            {
                var attributeValues = new List<DataAccess.Models.ProductAttributeCategoryValue.ProductAttributeCategoryValueEntityModel>();
                attr.ProductAttributeCategoryValue.ForEach(attrValue =>
                {
                    attributeValues.Add(new DataAccess.Models.ProductAttributeCategoryValue.ProductAttributeCategoryValueEntityModel
                    {
                        ProductAttributeCategoryValue1 = attrValue.ProductAttributeCategoryValue1,
                        ProductAttributeCategoryId = attrValue.ProductAttributeCategoryId,
                        CreatedById = attrValue.CreatedById,
                        CreatedDate = attrValue.CreatedDate,
                        UpdatedById = attrValue.UpdatedById,
                        UpdatedDate = attrValue.UpdatedDate,
                        Active = attrValue.Active
                    });
                });
                listProductAttributeCategory.Add(new DataAccess.Models.ProductAttributeCategory.ProductAttributeCategoryEntityModel
                {
                    ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                    ProductAttributeCategoryName = attr.ProductAttributeCategoryName,
                    CreatedById = attr.CreatedById,
                    CreatedDate = attr.CreatedDate,
                    UpdatedById = attr.UpdatedById,
                    UpdatedDate = attr.UpdatedDate,
                    Active = attr.Active,
                    ProductAttributeCategoryValue = attributeValues
                });        
            });

            var newListStartQuantityProduct = new List<DataAccess.Models.Product.ProductQuantityInWarehouseEntityModel>();
            ListInventoryReport.ForEach(product =>
            {
                var newListSerialByProduct = new List<DataAccess.Models.Product.ProductSerialInWarehouseEntityModel>();
                product.ListSerial.ForEach(serial =>
                {
                    newListSerialByProduct.Add(new DataAccess.Models.Product.ProductSerialInWarehouseEntityModel
                    {
                        SerialId = serial.SerialId,
                        SerialCode = serial.SerialCode,
                        ProductId = serial.ProductId,
                        StatusId = serial.StatusId,
                        WarehouseId = serial.WarehouseId,
                        Active = serial.Active,
                        CreatedDate = serial.CreatedDate,
                        CreatedById = serial.CreatedById,
                        UpdatedDate = serial.UpdatedDate,
                        UpdatedById = serial.UpdatedById
                    });
                });

                newListStartQuantityProduct.Add(new DataAccess.Models.Product.ProductQuantityInWarehouseEntityModel
                {
                    InventoryReportId = product.InventoryReportId,
                    WarehouseId = product.WarehouseId,
                    ProductId = product.ProductId,
                    Quantity = product.Quantity,
                    QuantityMinimum = product.QuantityMinimum,
                    QuantityMaximum = product.QuantityMaximum,
                    Active = product.Active,
                    Note = product.Note,
                    CreatedDate = product.CreatedDate,
                    CreatedById = product.CreatedById,
                    UpdatedDate = product.UpdatedDate,
                    UpdatedById = product.UpdatedById,
                    StartQuantity = product.StartQuantity,
                    OpeningBalance = product.OpeningBalance,
                    ListSerial = newListSerialByProduct
                });
            });

            var listProductVendorMapping = new List<DataAccess.Models.Product.ProductVendorMappingEntityModel>();
            ListProductVendorMapping.ForEach(item =>
            {
                listProductVendorMapping.Add(new DataAccess.Models.Product.ProductVendorMappingEntityModel
                {
                    VendorId = item.VendorId,
                    VendorProductName = item.VendorProductName,
                    MiniumQuantity = item.MiniumQuantity,
                    FromDate = item.FromDate,
                    ToDate = item.ToDate,
                    OrderNumber = item.OrderNumber,
                    MoneyUnitId = item.MoneyUnitId,
                    Price = item.Price
                });
            });

            return new CreateProductParameter
            {
                Product = Product,
                ListProductVendorMapping = listProductVendorMapping,
                ListProductAttributeCategory = listProductAttributeCategory,
                ListProductBillOfMaterials = ListProductBillOfMaterials,
                ListInventoryReport = newListStartQuantityProduct,
                ListProductImage = ListProductImage,
                UserId = UserId
            };
        }
    }
}
