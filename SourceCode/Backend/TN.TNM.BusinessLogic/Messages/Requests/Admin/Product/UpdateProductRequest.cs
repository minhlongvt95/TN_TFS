using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.BusinessLogic.Models.ProductAttributeCategory;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Product
{
    public class UpdateProductRequest : BaseRequest<UpdateProductParameter>
    {
        public ProductModel Product { get; set; }
        public List<ProductAttributeCategoryModel> lstProductAttributeCategory { get; set; }
        public List<DataAccess.Models.Product.ProductVendorMappingEntityModel> ListProductVendorMapping { get; set; }
        public Guid UserId { get; set; }
        public List<DataAccess.Models.Product.ProductQuantityInWarehouseEntityModel> ListInventoryReport { get; set; }
        public List<DataAccess.Models.Product.ProductBillOfMaterialsEntityModel> ListProductBillOfMaterials { get; set; }

        public override UpdateProductParameter ToParameter()
        {
            List<ProductAttributeCategory> productAttributeCategoryList = new List<ProductAttributeCategory>();

            lstProductAttributeCategory.ForEach(item =>
            {
                List<ProductAttributeCategoryValue> productAttributeCategoryValueList = new List<ProductAttributeCategoryValue>();

                item.ProductAttributeCategoryValue.ForEach(itemProductAttributeCategoryValue => {
                    productAttributeCategoryValueList.Add(itemProductAttributeCategoryValue.ToEntity());
                });
                var itemConvert = item.ToEntity();
                itemConvert.ProductAttributeCategoryValue = productAttributeCategoryValueList;
                productAttributeCategoryList.Add(itemConvert);
            });

            return new UpdateProductParameter
            {
                Product = Product.ToEntity(),
                ListProductVendorMapping = ListProductVendorMapping,
                lstProductAttributeCategory = productAttributeCategoryList,
                ListInventoryReport = ListInventoryReport,
                ListProductBillOfMaterials = ListProductBillOfMaterials,
                UserId = UserId
            };
        }
    }
}
