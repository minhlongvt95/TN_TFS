using System;
using System.Collections.Generic;
using System.Linq;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;
using TN.TNM.DataAccess.Messages.Results.Admin.Product;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.Product;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class ProductDAO : BaseDAO, IProductDataAccess
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public ProductDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment hostingEnvironment)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            _hostingEnvironment = hostingEnvironment;
        }
        public SearchProductResult SearchProduct(SearchProductParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.SEARCH, ObjectName.PRODUCT, "Search product", parameter.UserId);
                var commonProductCategory = context.ProductCategory.ToList();
                var commonProduct = context.Product.ToList();
                var commonProductVendorMapping = context.ProductVendorMapping.ToList();
                var commonCategoryType = context.CategoryType.ToList();
                var commonCategory = context.Category.ToList();

                var productUnitTypeId = commonCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DNH")?.CategoryTypeId;
                var listAllProductUnit = commonCategory.Where(c => c.CategoryTypeId == productUnitTypeId).ToList() ?? new List<Category>();

                var propertyTypeId = commonCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TC")?.CategoryTypeId;
                var listProperty = commonCategory.Where(c => c.CategoryTypeId == propertyTypeId).ToList() ?? new List<Category>();

                var caculatorInventoryPriceTypeId = commonCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "GTK")?.CategoryTypeId;
                var listCaculatorInventoryPrice = commonCategory.Where(c => c.CategoryTypeId == caculatorInventoryPriceTypeId).ToList() ?? new List<Category>();

                #region Kiểm tra các references của Product
                var vendorOrderDetails = context.VendorOrderDetail.ToList();
                var customerOrderDetails = context.CustomerOrderDetail.ToList();
                var quoteDetails = context.QuoteDetail.ToList();
                var procurementRequestItems = context.ProcurementRequestItem.ToList();

                #endregion

                if (parameter.ListProductCategory.Count > 0)
                {
                    List<Guid> listGuidTemp = parameter.ListProductCategory;
                    for (int i = 0; i < listGuidTemp.Count; ++i)
                    {
                        ListChildProductCategory(listGuidTemp[i], parameter.ListProductCategory, commonProductCategory);
                    }
                }
                var listVendorMappingSearch = context.ProductVendorMapping.Where(c => parameter.ListVendor.Count == 0 || parameter.ListVendor == null ||
                                                    parameter.ListVendor.Contains(c.VendorId)).Select(c => c.ProductId).ToList();


                var productList = commonProduct.Where(c => c.Active == true && (parameter.ListVendor == null || parameter.ListVendor.Count == 0 || listVendorMappingSearch.Contains(c.ProductId)) &&
                                 (parameter.ProductName == null || parameter.ProductName == "" || c.ProductName.ToLower().Contains(parameter.ProductName.ToLower().Trim())) &&
                                 (parameter.ProductCode == null || parameter.ProductCode == "" || c.ProductCode.ToLower().Contains(parameter.ProductCode.ToLower().Trim())) &&
                                 (parameter.ListProductCategory.Contains(c.ProductCategoryId) || parameter.ListProductCategory.Count == 0))
                                  .Select(m => new ProductEntityModel
                                  {
                                      ProductId = m.ProductId,
                                      ProductCategoryId = m.ProductCategoryId,
                                      ProductName = m.ProductName,
                                      ProductCode = m.ProductCode,
                                      ProductDescription = m.ProductDescription,
                                      ProductUnitId = m.ProductUnitId,
                                      Quantity = m.Quantity,
                                      Price1 = m.Price1,
                                      Price2 = m.Price2,
                                      Active = m.Active,
                                      CreatedById = m.CreatedById,
                                      CreatedDate = m.CreatedDate,
                                      UpdatedById = m.UpdatedById,
                                      UpdatedDate = m.UpdatedDate,
                                      ProductCategoryName = commonProductCategory.FirstOrDefault(c => c.ProductCategoryId == m.ProductCategoryId)?.ProductCategoryName ?? "",
                                      MinimumInventoryQuantity = m.MinimumInventoryQuantity,
                                      GuaranteeTime = m.GuaranteeTime,
                                      ProductUnitName = listAllProductUnit.FirstOrDefault(c => c.CategoryId == m.ProductUnitId)?.CategoryName ?? "",
                                      PropertyName = listProperty.FirstOrDefault(c => c.CategoryId == m.PropertyId)?.CategoryName ?? "",
                                      CalculateInventoryPricesName = listCaculatorInventoryPrice.FirstOrDefault(c => c.CategoryId == m.CalculateInventoryPricesId)?.CategoryName ?? "",

                                      CountProductInformation = CountProductInformation(
                                                                       m.ProductId,
                                                                       vendorOrderDetails,
                                                                       customerOrderDetails,
                                                                       quoteDetails,
                                                                       procurementRequestItems),
                                      //ListVendorName= getListNameVendor(m.ProductId),

                                  }).ToList();
                var resultGroup = productList.GroupBy(x => x.ProductId).Select(y => y.First()).ToList();
                resultGroup.ForEach(item =>
                {
                    item.ListVendorName = getListNameVendor(item.ProductId);
                });

                //sort by created date desc
                resultGroup = resultGroup.OrderByDescending(w => w.CreatedDate).ToList();
                return new SearchProductResult
                {
                    Status = true,
                    ProductList = resultGroup
                };
            }
            catch (Exception ex)
            {
                return new SearchProductResult
                {
                    Status = false,
                    Message = ex.ToString()
                };
            }
        }
        public void ListChildProductCategory(Guid ProductCategoryID, List<Guid> listResult, List<ProductCategory> commonProductCategory)
        {
            var listProductCategoryChil = commonProductCategory.Where(item => item.ParentId == ProductCategoryID).ToList();
            if (listProductCategoryChil.Count > 0)
            {
                for (int i = 0; i < listProductCategoryChil.Count; ++i)
                {
                    listResult.Add(listProductCategoryChil[i].ProductCategoryId);
                    ListChildProductCategory(listProductCategoryChil[i].ProductCategoryId, listResult, commonProductCategory);
                }
            }
        }
        public CreateProductResult CreateProduct(CreateProductParameter parameter)
        {
            try
            {
                #region Add Product
                var productId = Guid.NewGuid();
                var newProduct = new Databases.Entities.Product
                {
                    ProductId = productId,
                    ProductCategoryId = parameter.Product.ProductCategoryId,
                    ProductName = parameter.Product.ProductName.Trim(),
                    ProductCode = parameter.Product.ProductCode.Trim(),
                    Price1 = parameter.Product.Price1,
                    CreatedDate = DateTime.Now,
                    ProductUnitId = parameter.Product.ProductUnitId,
                    ProductDescription = parameter.Product.ProductDescription?.Trim(),
                    Vat = parameter.Product.Vat,
                    ProductMoneyUnitId = parameter.Product.ProductMoneyUnitId,
                    GuaranteeTime = parameter.Product.GuaranteeTime,
                    ExWarehousePrice = parameter.Product.ExWarehousePrice,
                    CreatedById = parameter.UserId,
                    //default values
                    UpdatedById = null,
                    Price2 = 0,
                    UpdatedDate = null,
                    Active = true,
                    Quantity = 0,
                    Guarantee = null,
                    GuaranteeDatetime = null,
                    MinimumInventoryQuantity = 0, //trường số lượng tồn kho tối thiểu chuyển qua dùng ở bảng InventoryReport, trường QuantityMinimun
                    CalculateInventoryPricesId = parameter.Product.CalculateInventoryPricesId,
                    PropertyId = parameter.Product.PropertyId,
                    WarehouseAccountId = parameter.Product.WarehouseAccountId,
                    RevenueAccountId = parameter.Product.RevenueAccountId,
                    PayableAccountId = parameter.Product.PayableAccountId,
                    ImportTax = parameter.Product.ImportTax,
                    CostPriceAccountId = parameter.Product.CostPriceAccountId,
                    AccountReturnsId = parameter.Product.AccountReturnsId,
                    FolowInventory = parameter.Product.FolowInventory,
                    ManagerSerialNumber = parameter.Product.ManagerSerialNumber,
                };
                context.Product.Add(newProduct);

                var productResponse = new DataAccess.Models.Product.ProductEntityModel()
                {
                    ProductId = newProduct.ProductId,
                    ProductName = newProduct.ProductName,
                    ProductCode = newProduct.ProductCode,

                };

                #endregion

                #region Add Mapping Product and Vendors
                if (parameter.ListProductVendorMapping.Count > 0)
                {
                    var listProductVendorMapping = new List<ProductVendorMapping>();
                    parameter.ListProductVendorMapping.ForEach(vendor =>
                    {
                        var productVendorObj = new ProductVendorMapping
                        {
                            ProductVendorMappingId = Guid.NewGuid(),
                            ProductId = productId,
                            VendorId = vendor.VendorId,
                            VendorProductName = vendor.VendorProductName,
                            MiniumQuantity = vendor.MiniumQuantity,
                            UnitPriceId = vendor.MoneyUnitId,
                            Price = vendor.Price,
                            FromDate = vendor.FromDate,
                            ToDate = vendor.ToDate,
                            OrderNumber = vendor.OrderNumber,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedById = null,
                            UpdatedDate = null,
                            Active = true
                        };
                        listProductVendorMapping.Add(productVendorObj);
                    });
                    context.ProductVendorMapping.AddRange(listProductVendorMapping);
                }
                #endregion

                #region Add Product Attribute             
                if (parameter.ListProductAttributeCategory.Count > 0)
                {
                    var listAttributeCategory = new List<Databases.Entities.ProductAttributeCategory>();
                    var listProductAttributeCategoryValue = new List<Databases.Entities.ProductAttributeCategoryValue>();
                    var listProductAttribute = new List<Databases.Entities.ProductAttribute>();
                    parameter.ListProductAttributeCategory.ForEach(attribute =>
                    {
                        //định nghĩa product attribute category
                        var newAttributeCategoryId = Guid.NewGuid();
                        var attributeCategoryObj = new Databases.Entities.ProductAttributeCategory
                        {
                            ProductAttributeCategoryId = newAttributeCategoryId,
                            ProductAttributeCategoryName = attribute.ProductAttributeCategoryName?.Trim(),
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedById = null,
                            UpdatedDate = null,
                            Active = true
                        };
                        listAttributeCategory.Add(attributeCategoryObj);
                        //gắn category với sản phẩm
                        var productAttribute = new Databases.Entities.ProductAttribute
                        {
                            ProductAttributeId = Guid.NewGuid(),
                            ProductId = productId,
                            ProductAttributeCategoryId = newAttributeCategoryId,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = null,
                            Active = true,
                            UpdatedBy = null,
                            CreatedBy = parameter.UserId
                        };
                        listProductAttribute.Add(productAttribute);
                        //định nghĩa product attribute value
                        if (attribute.ProductAttributeCategoryValue.Count > 0)
                        {
                            attribute.ProductAttributeCategoryValue.ForEach(attriButeValue =>
                            {
                                var attributeValue = new Databases.Entities.ProductAttributeCategoryValue
                                {
                                    ProductAttributeCategoryValueId = Guid.NewGuid(),
                                    ProductAttributeCategoryValue1 = attriButeValue.ProductAttributeCategoryValue1?.Trim(),
                                    ProductAttributeCategoryId = newAttributeCategoryId,
                                    CreatedById = parameter.UserId,
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = null,
                                    UpdatedById = null,
                                    Active = true
                                };
                                listProductAttributeCategoryValue.Add(attributeValue);
                            });
                        }
                    });
                    context.ProductAttributeCategory.AddRange(listAttributeCategory);
                    context.ProductAttributeCategoryValue.AddRange(listProductAttributeCategoryValue);
                    context.ProductAttribute.AddRange(listProductAttribute);
                }
                #endregion

                #region Add Product BOM
                var listProductBOM = new List<ProductBillOfMaterials>();

                parameter.ListProductBillOfMaterials?.ForEach(bom =>
                {
                    listProductBOM.Add(new ProductBillOfMaterials()
                    {
                        ProductBillOfMaterialId = Guid.NewGuid(),
                        ProductId = newProduct.ProductId, //lấy theo id sản phẩm vừa tạo
                        ProductMaterialId = bom.ProductMaterialId,
                        Quantity = bom.Quantity,
                        EffectiveFromDate = bom.EffectiveFromDate,
                        EffectiveToDate = bom.EffectiveToDate,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now
                    });
                });

                context.ProductBillOfMaterials.AddRange(listProductBOM);
                #endregion

                #region Thêm vào bảng Báo cáo tồn kho

                var SERIAL_STATUS_CODE = "TSE";
                var serialStatusId = context.CategoryType
                    .FirstOrDefault(w => w.CategoryTypeCode == SERIAL_STATUS_CODE)?.CategoryTypeId;
                var NEW_SERIAL_STATUS_CODE = "CXU"; //Mặc định trạng thái mới của serial: Chưa xuất;
                var statusId = context.Category.FirstOrDefault(w =>
                        w.CategoryTypeId == serialStatusId && w.CategoryCode == NEW_SERIAL_STATUS_CODE)?
                    .CategoryId;

                if (parameter.ListInventoryReport.Count > 0)
                {
                    var listInventoryReport = new List<InventoryReport>();
                    var listSerial = new List<Serial>();
                    parameter.ListInventoryReport.ForEach(item =>
                    {
                        var newInventoryReportObj = new InventoryReport
                        {
                            InventoryReportId = Guid.NewGuid(),
                            WarehouseId = item.WarehouseId,
                            ProductId = productId,
                            Quantity = 0, //mặc định 0
                            QuantityMinimum = item.QuantityMinimum,
                            StartQuantity = item.StartQuantity ?? 0,
                            QuantityMaximum = item.QuantityMaximum,
                            OpeningBalance = item.OpeningBalance ?? 0,
                            Note = item.Note,
                            Active = true,
                            CreatedDate = DateTime.Now,
                            CreatedById = parameter.UserId
                        };
                        listInventoryReport.Add(newInventoryReportObj);

                        #region Add Serial 

                        if (item.ListSerial.Count > 0)
                        {
                            item.ListSerial.ForEach(serial =>
                            {
                                var newSerial = new Serial
                                {
                                    SerialId = Guid.NewGuid(),
                                    SerialCode = serial.SerialCode?.Trim(),
                                    ProductId = productId,
                                    StatusId = statusId.Value,
                                    WarehouseId = item.WarehouseId,
                                    CreatedDate = DateTime.Now,
                                    Active = true,
                                    CreatedById = parameter.UserId,
                                    UpdatedDate = null,
                                    UpdatedById = null,
                                };
                                listSerial.Add(newSerial);
                            });
                        }

                        #endregion
                    });

                    context.InventoryReport.AddRange(listInventoryReport);
                    context.Serial.AddRange(listSerial);
                }

                #endregion

                #region Add Product Image

                if (parameter.ListProductImage.Count > 0)
                {
                    var listProductImage = new List<ProductImage>();
                    string folderName = "ProductImage";
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
                    parameter.ListProductImage.ForEach(image =>
                    {
                        listProductImage.Add(new ProductImage
                        {
                            ProductImageId = Guid.NewGuid(),
                            ProductId = productId,
                            ImageName = image.ImageName.Trim(),
                            ImageSize = image.ImageSize,
                            ImageUrl = Path.Combine(newPath, image.ImageName),
                            //default values 
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedById = parameter.UserId,
                            UpdatedDate = DateTime.Now,
                        });
                    });
                    context.ProductImage.AddRange(listProductImage);
                }

                #endregion

                context.SaveChanges();

                return new CreateProductResult
                {
                    Status = true,
                    Message = "Tạo sản phẩm/dịch vụ thành công",
                    ProductId = productId,
                    NewProduct = productResponse
                };
            }
            catch (Exception ex)
            {
                return new CreateProductResult
                {
                    Status = false,
                    Message = ex.ToString()
                };
            }
        }

        public ImportProductResult ImportProduct(ImportProductParameter parameter)
        {
            var products = new List<Product>();
            parameter.ListProduct.ForEach(item =>
            {
                var newProduct = new Databases.Entities.Product
                {
                    ProductId = Guid.NewGuid(),
                    ProductCategoryId = item.ProductCategoryId,
                    ProductName = item.ProductName.Trim(),
                    ProductCode = item.ProductCode.Trim(),
                    Price1 = item.Price1,
                    CreatedDate = DateTime.Now,
                    ProductUnitId = item.ProductUnitId,
                    ProductDescription = item.ProductDescription?.Trim(),
                    Vat = item.Vat,
                    ProductMoneyUnitId = item.ProductMoneyUnitId,
                    GuaranteeTime = item.GuaranteeTime,
                    ExWarehousePrice = item.ExWarehousePrice,
                    CreatedById = parameter.UserId,
                    //default values
                    UpdatedById = null,
                    Price2 = 0,
                    UpdatedDate = null,
                    Active = true,
                    Quantity = 0,
                    Guarantee = null,
                    GuaranteeDatetime = null,
                    MinimumInventoryQuantity = 0, //trường số lượng tồn kho tối thiểu chuyển qua dùng ở bảng InventoryReport, trường QuantityMinimun
                    CalculateInventoryPricesId = item.CalculateInventoryPricesId,
                    PropertyId = item.PropertyId,
                    WarehouseAccountId = item.WarehouseAccountId,
                    RevenueAccountId = item.RevenueAccountId,
                    PayableAccountId = item.PayableAccountId,
                    ImportTax = item.ImportTax,
                    CostPriceAccountId = item.CostPriceAccountId,
                    AccountReturnsId = item.AccountReturnsId,
                    FolowInventory = item.FolowInventory,
                    ManagerSerialNumber = item.ManagerSerialNumber,
                };
                products.Add(newProduct);
            });
            context.Product.AddRange(products);
            context.SaveChanges();

            return new ImportProductResult
            {
                Status = true,
                Message = CommonMessage.Product.IMPORT_SUCCESS,
            };
        }

        public GetProductByIDResult GetProductByID(GetProductByIDParameter parameter)
        {
            try
            {
                #region Add by Dung
                var productResponse = new Product();
                var Product = context.Product.Where(m => m.ProductId == parameter.ProductId).FirstOrDefault();
                if (Product != null)
                {
                    productResponse.ProductId = Product.ProductId;
                    productResponse.ProductCategoryId = Product.ProductCategoryId;
                    productResponse.ProductName = Product.ProductName;
                    productResponse.ProductCode = Product.ProductCode;
                    productResponse.Price1 = Product.Price1;
                    productResponse.Price2 = Product.Price2;
                    productResponse.ProductDescription = Product.ProductDescription;
                    productResponse.Vat = Product.Vat;
                    productResponse.ProductMoneyUnitId = Product.ProductMoneyUnitId;
                    productResponse.ProductUnitId = Product.ProductUnitId;
                    productResponse.GuaranteeTime = Product.GuaranteeTime;
                    productResponse.ExWarehousePrice = Product.ExWarehousePrice ?? 0;
                    productResponse.CalculateInventoryPricesId = Product.CalculateInventoryPricesId;
                    productResponse.PropertyId = Product.PropertyId;
                    productResponse.WarehouseAccountId = Product.WarehouseAccountId;
                    productResponse.RevenueAccountId = Product.RevenueAccountId;
                    productResponse.PayableAccountId = Product.PayableAccountId;
                    productResponse.ImportTax = Product.ImportTax;
                    productResponse.CostPriceAccountId = Product.CostPriceAccountId;
                    productResponse.AccountReturnsId = Product.AccountReturnsId;
                    productResponse.FolowInventory = Product.FolowInventory;
                    productResponse.ManagerSerialNumber = Product.ManagerSerialNumber;
                }
                var listVendorMapping = context.ProductVendorMapping.Where(c => c.ProductId == parameter.ProductId).OrderBy(x => x.OrderNumber).ToList();
                var listVendorMappingId = listVendorMapping.Select(w => w.VendorId).ToList();
                //var listVendor = new List<Vendor>();
                //var listVendorByProductId = context.Vendor.Where(w => listVendorMappingId.Contains(w.VendorId) && w.Active == true).Select(w => new
                //{
                //    w.VendorId,
                //    w.VendorName,
                //    w.VendorCode,
                //}).ToList();
                //listVendorByProductId.ForEach(vendor =>
                //{
                //    listVendor.Add(new Vendor
                //    {
                //        VendorId = vendor.VendorId,
                //        VendorName = vendor.VendorName,
                //        VendorCode = vendor.VendorCode
                //    });
                //});

                var listProductAttributeCategory = new List<ProductAttributeCategory>();
                //get mapping product with attribute
                var listAttributeId = context.ProductAttribute.Where(w => w.ProductId == parameter.ProductId).Select(w => w.ProductAttributeCategoryId).ToList();
                if (listAttributeId != null)
                {
                    //get name of attribute 
                    var listProductAttributeCategoryEntity = context.ProductAttributeCategory.Where(w => listAttributeId.Contains(w.ProductAttributeCategoryId)).ToList();
                    var listAttributeCategoryIdEntity = listProductAttributeCategoryEntity.Select(w => w.ProductAttributeCategoryId).ToList();
                    var listAttributeValueEntity = context.ProductAttributeCategoryValue.Where(w => listAttributeCategoryIdEntity.Contains(w.ProductAttributeCategoryId)).ToList();

                    //get value of attribute
                    listProductAttributeCategoryEntity.ForEach(productAttributeCategory =>
                    {
                        var newListAttribute = new List<ProductAttributeCategoryValue>();
                        var listAttributeValueByCategory = listAttributeValueEntity.Where(w => w.ProductAttributeCategoryId == productAttributeCategory.ProductAttributeCategoryId).ToList();
                        listAttributeValueByCategory.ForEach(attValue =>
                        {
                            newListAttribute.Add(new ProductAttributeCategoryValue
                            {
                                ProductAttributeCategoryValueId = attValue.ProductAttributeCategoryValueId,
                                ProductAttributeCategoryValue1 = attValue.ProductAttributeCategoryValue1,
                                ProductAttributeCategoryId = attValue.ProductAttributeCategoryId
                            });
                        });

                        listProductAttributeCategory.Add(new ProductAttributeCategory
                        {
                            ProductAttributeCategoryId = productAttributeCategory.ProductAttributeCategoryId,
                            ProductAttributeCategoryName = productAttributeCategory.ProductAttributeCategoryName,
                            ProductAttributeCategoryValue = newListAttribute
                        });
                    });
                }
                #endregion

                #region Get Customer Order
                var listCustomerOrder = new List<CustomerOrderEntityModel>();
                var customerOrderDetail = context.CustomerOrderDetail.Where(w => w.ProductId == parameter.ProductId).ToList();
                if (customerOrderDetail != null)
                {
                    //get list order by order detail
                    var listOrderDetailId = customerOrderDetail.Select(w => w.OrderId).ToList();
                    var listOrder = context.CustomerOrder.Where(w => listOrderDetailId.Contains(w.OrderId)).ToList();
                    var listCustomerOrderId = listOrder.Select(w => w.CustomerId).ToList();
                    var listCustomer = context.Customer.Where(w => listCustomerOrderId.Contains(w.CustomerId)).ToList();

                    customerOrderDetail.ForEach(orderdetail =>
                    {
                        var order = listOrder.Where(w => w.OrderId == orderdetail.OrderId).FirstOrDefault();
                        var customer = listCustomer.Where(w => w.CustomerId == order.CustomerId).FirstOrDefault();

                        listCustomerOrder.Add(new CustomerOrderEntityModel
                        {
                            OrderId = orderdetail.OrderId,
                            CustomerName = customer.CustomerName?.Trim(),
                            CustomerId = customer.CustomerId,
                            OrderCode = order.OrderCode?.Trim(),
                            OrderDate = order.OrderDate,
                            //default values
                            Description = null,
                            Note = null,
                            PaymentMethod = Guid.Empty,
                            DaysAreOwed = null,
                            MaxDebt = null,
                            ReceivedDate = null,
                            ReceivedHour = null,
                            RecipientName = null,
                            LocationOfShipment = null,
                            ShippingNote = null,
                            RecipientPhone = null,
                            RecipientEmail = null,
                            PlaceOfDelivery = null,
                            Amount = 0,
                            DiscountValue = null,
                            StatusId = null,
                            CreatedById = Guid.Empty,
                            CreatedDate = DateTime.Now,
                            UpdatedById = null,
                            UpdatedDate = null,
                            Active = null
                        });
                    });
                }
                #endregion

                #region Get Product Image
                var ListProductImage = new List<DataAccess.Databases.Entities.ProductImage>();

                var productImageEntity = context.ProductImage.Where(w => w.Active == true && w.ProductId == parameter.ProductId).ToList();
                if (productImageEntity != null)
                {
                    productImageEntity.ForEach(option =>
                    {
                        ListProductImage.Add(new ProductImage
                        {
                            ProductImageId = option.ProductImageId,
                            ProductId = option.ProductId,
                            ImageName = option.ImageName,
                            ImageSize = option.ImageSize,
                            ImageUrl = option.ImageUrl,
                        });
                    });
                }

                #endregion

                #region Lấy tồn kho đầu kỳ và tồn kho tối thiểu

                var listInventory = new List<InventoryReportByProductIdEntityModel>();

                var inventoryByProductId =
                    context.InventoryReport.Where(w => w.ProductId == parameter.ProductId).ToList();

                inventoryByProductId?.ForEach(inventory =>
                {
                    listInventory.Add(new InventoryReportByProductIdEntityModel
                    {
                        InventoryReportId = inventory.InventoryReportId,
                        WarehouseId = inventory.WarehouseId,
                        ProductId = inventory.ProductId,
                        Quantity = inventory.Quantity,
                        QuantityMinimum = inventory.QuantityMinimum,
                        QuantityMaximum = inventory.QuantityMaximum,
                        StartQuantity = inventory.StartQuantity,
                        OpeningBalance = inventory.OpeningBalance,
                        Note = inventory.Note,
                        ListSerial = new List<SerialEntityModel>()
                    });
                });

                if (listInventory.Count > 0)
                {
                    var listAllSerial = context.Serial.ToList();

                    listInventory.ForEach(item =>
                    {
                        var listSerial = listAllSerial
                            .Where(x => x.WarehouseId == item.WarehouseId && x.ProductId == item.ProductId).Select(y =>
                                new SerialEntityModel
                                {
                                    SerialId = y.SerialId,
                                    SerialCode = y.SerialCode,
                                    ProductId = y.ProductId,
                                    WarehouseId = y.WarehouseId,
                                    StatusId = y.StatusId,
                                    CreatedDate = y.CreatedDate
                                }).ToList();

                        item.ListSerial = listSerial;
                    });
                }

                #endregion

                #region Lấy Product BOM
                var productBOMEntity = context.ProductBillOfMaterials.Where(w => w.Active == true && w.ProductId == parameter.ProductId).ToList();
                var listProductEntity = context.Product.ToList();
                var unitTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "DNH").CategoryTypeId;
                var listProductUnit = context.Category.Where(w => w.Active == true && w.CategoryTypeId == unitTypeId).ToList();

                var listProductBOM = new List<DataAccess.Models.Product.ProductBillOfMaterialsEntityModel>();
                productBOMEntity?.ForEach(bom =>
                {
                    var productMaterial = listProductEntity.FirstOrDefault(f => f.ProductId == bom.ProductMaterialId);
                    var productName = productMaterial?.ProductName ?? "";
                    var productCode = productMaterial?.ProductCode ?? "";
                    var productUnitName = listProductUnit.FirstOrDefault(f => productMaterial.ProductUnitId == f.CategoryId)?.CategoryName ?? "";

                    listProductBOM.Add(new ProductBillOfMaterialsEntityModel()
                    {
                        ProductBillOfMaterialId = bom.ProductBillOfMaterialId,
                        ProductId = bom.ProductId,
                        ProductMaterialId = bom.ProductMaterialId,
                        Quantity = bom.Quantity,
                        EffectiveFromDate = bom.EffectiveFromDate,
                        EffectiveToDate = bom.EffectiveToDate,
                        ProductName = productName,
                        ProductCode = productCode,
                        ProductUnitName = productUnitName
                    });
                });
                #endregion

                #region Kiểm tra điều kiện xóa sản phẩm
                var CanDelete = true;
                var checkVendorOrderDetail = context.VendorOrderDetail.FirstOrDefault(f => f.ProductId == parameter.ProductId);
                var checkCustomerOrderDetail = context.CustomerOrderDetail.FirstOrDefault(f => f.ProductId == parameter.ProductId);
                var checkQuoteDetail = context.QuoteDetail.FirstOrDefault(f => f.ProductId == parameter.ProductId);
                var checkProcurementRequestItem = context.ProcurementRequestItem.FirstOrDefault(f => f.ProductId == parameter.ProductId);

                if (checkVendorOrderDetail != null || checkCustomerOrderDetail != null || checkQuoteDetail != null || checkProcurementRequestItem != null)
                {
                    CanDelete = false;
                }
                #endregion

                return new GetProductByIDResult
                {
                    Product = productResponse,
                    lstProductAttributeCategory = listProductAttributeCategory,
                    LstProductVendorMapping = listVendorMapping,
                    lstCustomerOrder = listCustomerOrder.OrderByDescending(w => w.OrderDate).ToList(),
                    ListProductImage = ListProductImage,
                    ListInventory = listInventory,
                    Status = true,
                    CanDelete = CanDelete,
                    ListProductBillOfMaterials = listProductBOM
                    //CountProductInformation = countProductInformation
                };
            }
            catch (Exception ex)
            {
                return new GetProductByIDResult
                {
                    Status = false,
                    Message = ex.ToString(),
                };

            }
        }

        public UpdateProductResult UpdateProduct(UpdateProductParameter parameter)
        {
            try
            {
                #region Update Product
                var product = context.Product.Where(w => w.ProductId == parameter.Product.ProductId).FirstOrDefault();
                if (product != null)
                {
                    product.ProductCategoryId = parameter.Product.ProductCategoryId;
                    product.ProductCode = parameter.Product.ProductCode.Trim();
                    product.ProductName = parameter.Product.ProductName.Trim();
                    product.Price1 = parameter.Product.Price1;
                    product.ExWarehousePrice = parameter.Product.ExWarehousePrice;
                    product.ProductUnitId = parameter.Product.ProductUnitId;
                    product.ProductMoneyUnitId = parameter.Product.ProductMoneyUnitId;
                    product.Vat = parameter.Product.Vat;
                    product.GuaranteeTime = parameter.Product.GuaranteeTime;
                    product.ProductDescription = parameter.Product.ProductDescription?.Trim();
                    product.UpdatedById = parameter.UserId;
                    product.UpdatedDate = DateTime.Now;
                    product.MinimumInventoryQuantity = 0; //trường số lượng tồn kho tối thiểu chuyển qua dùng ở bảng InventoryReport, trường QuantityMinimun
                    product.CalculateInventoryPricesId = parameter.Product.CalculateInventoryPricesId;
                    product.PropertyId = parameter.Product.PropertyId;
                    product.WarehouseAccountId = parameter.Product.WarehouseAccountId;
                    product.RevenueAccountId = parameter.Product.RevenueAccountId;
                    product.PayableAccountId = parameter.Product.PayableAccountId;
                    product.ImportTax = parameter.Product.ImportTax;
                    product.CostPriceAccountId = parameter.Product.CostPriceAccountId;
                    product.AccountReturnsId = parameter.Product.AccountReturnsId;
                    product.FolowInventory = parameter.Product.FolowInventory;
                    product.ManagerSerialNumber = parameter.Product.ManagerSerialNumber;

                    context.Product.Update(product);
                }
                #endregion

                #region Update Product Mapping Vendor
                //delete old records
                var oldList = context.ProductVendorMapping.Where(w => w.ProductId == parameter.Product.ProductId).ToList();
                if (oldList != null)
                {
                    context.ProductVendorMapping.RemoveRange(oldList);
                }
                var newList = new List<ProductVendorMapping>();
                parameter.ListProductVendorMapping.ForEach(vendor =>
                {
                    newList.Add(new ProductVendorMapping
                    {
                        ProductVendorMappingId = Guid.NewGuid(),
                        ProductId = parameter.Product.ProductId,
                        VendorId = vendor.VendorId,
                        VendorProductName = vendor.VendorProductName,
                        MiniumQuantity = vendor.MiniumQuantity,
                        UnitPriceId = vendor.MoneyUnitId,
                        Price = vendor.Price,
                        FromDate = vendor.FromDate,
                        ToDate = vendor.ToDate,
                        OrderNumber = vendor.OrderNumber,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        UpdatedById = null,
                        UpdatedDate = null,
                        Active = true
                    });
                });
                context.ProductVendorMapping.AddRange(newList);
                #endregion

                #region Update Attribute
                //remove old value
                var oldProductAttribute = context.ProductAttribute.Where(w => w.ProductId == parameter.Product.ProductId).ToList();
                context.ProductAttribute.RemoveRange(oldProductAttribute);
                context.SaveChanges();
                //var listOldCategoryId = oldProductAttribute.Select(w => w.ProductAttributeCategoryId).ToList();
                //var oldProductAttributeCategory = context.ProductAttributeCategory.Where(w => listOldCategoryId.Contains(w.ProductAttributeCategoryId)).ToList();
                //var listOldAttributeCategoryId = oldProductAttributeCategory.Select(w => w.ProductAttributeCategoryId).ToList();
                //context.ProductAttributeCategory.RemoveRange(oldProductAttributeCategory);
                //context.SaveChanges();
                //var oldProductAttributeCategoryValue = context.ProductAttributeCategoryValue.Where(w => listOldAttributeCategoryId.Contains(w.ProductAttributeCategoryId)).ToList();
                //context.ProductAttributeCategoryValue.RemoveRange(oldProductAttributeCategoryValue);

                //add new
                if (parameter.lstProductAttributeCategory.Count > 0)
                {
                    var listAttributeCategory = new List<Databases.Entities.ProductAttributeCategory>();
                    var listProductAttributeCategoryValue = new List<Databases.Entities.ProductAttributeCategoryValue>();
                    var listProductAttribute = new List<Databases.Entities.ProductAttribute>();
                    parameter.lstProductAttributeCategory.ForEach(attribute =>
                    {
                        //định nghĩa product attribute category
                        var newAttributeCategoryId = Guid.NewGuid();
                        var attributeCategoryObj = new Databases.Entities.ProductAttributeCategory
                        {
                            ProductAttributeCategoryId = newAttributeCategoryId,
                            ProductAttributeCategoryName = attribute.ProductAttributeCategoryName?.Trim(),
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedById = null,
                            UpdatedDate = null,
                            Active = true
                        };
                        listAttributeCategory.Add(attributeCategoryObj);
                        //gắn category với sản phẩm
                        var productAttribute = new Databases.Entities.ProductAttribute
                        {
                            ProductAttributeId = Guid.NewGuid(),
                            ProductId = parameter.Product.ProductId,
                            ProductAttributeCategoryId = newAttributeCategoryId,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = null,
                            Active = true,
                            UpdatedBy = null,
                            CreatedBy = parameter.UserId
                        };
                        listProductAttribute.Add(productAttribute);
                        //định nghĩa product attribute value
                        if (attribute.ProductAttributeCategoryValue.Count > 0)
                        {
                            List<ProductAttributeCategoryValue> listAttributeValue = attribute.ProductAttributeCategoryValue.ToList();

                            listAttributeValue.ForEach(attriButeValue =>
                            {
                                var attributeValue = new Databases.Entities.ProductAttributeCategoryValue
                                {
                                    ProductAttributeCategoryValueId = Guid.NewGuid(),
                                    ProductAttributeCategoryValue1 = attriButeValue.ProductAttributeCategoryValue1?.Trim(),
                                    ProductAttributeCategoryId = newAttributeCategoryId,
                                    CreatedById = parameter.UserId,
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = null,
                                    UpdatedById = null,
                                    Active = true
                                };
                                listProductAttributeCategoryValue.Add(attributeValue);
                            });
                        }
                    });
                    context.ProductAttributeCategory.AddRange(listAttributeCategory);
                    context.ProductAttributeCategoryValue.AddRange(listProductAttributeCategoryValue);
                    context.ProductAttribute.AddRange(listProductAttribute);
                }
                #endregion

                #region Update Inventory Report



                #endregion

                #region Update Product BOM
                //delete old product BOM
                var listOldBOM = context.ProductBillOfMaterials.Where(w => w.ProductId == parameter.Product.ProductId).ToList();
                context.ProductBillOfMaterials.RemoveRange(listOldBOM);
                //add new product BOM
                var listProductBOM = new List<ProductBillOfMaterials>();
                parameter.ListProductBillOfMaterials?.ForEach(bom =>
                {
                    listProductBOM.Add(new ProductBillOfMaterials()
                    {
                        ProductBillOfMaterialId = Guid.NewGuid(),
                        ProductId = parameter.Product.ProductId, //lấy theo id sản phẩm vừa update
                        ProductMaterialId = bom.ProductMaterialId,
                        Quantity = bom.Quantity,
                        EffectiveFromDate = bom.EffectiveFromDate,
                        EffectiveToDate = bom.EffectiveToDate,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now
                    });
                });
                context.ProductBillOfMaterials.AddRange(listProductBOM);
                #endregion

                context.SaveChanges();

                return new UpdateProductResult
                {
                    Status = true,
                    Message = "Chỉnh sửa sản phẩm/dịch vụ thành công",
                    ProductId = parameter.Product.ProductId
                };
            }
            catch (Exception ex)
            {
                return new UpdateProductResult
                {
                    Message = ex.ToString(),
                    Status = false
                };
            }
        }

        public string getListNameVendor(Guid ProductId)
        {
            string Result = string.Empty;
            var listVendorId = context.ProductVendorMapping.Where(c => c.ProductId == ProductId)?.Select(c => c.VendorId).ToList() ?? new List<Guid>();
            if (listVendorId.Count != 0)
            {
                var listVendor = context.Vendor.Where(c => listVendorId.Contains(c.VendorId)).Select(c => c.VendorName).ToList();
                Result = string.Join(";", listVendor);
            }
            else
            {
                Result = "";
            }

            return Result;
        }

        public GetProductByVendorIDResult GetGetProductByVendorID(GetProductByVendorIDParameter parameter)
        {
            try
            {
                var listProduct = (from product in context.Product
                                   join productvendormapping in context.ProductVendorMapping on product.ProductId equals
                                       productvendormapping.ProductId
                                   where productvendormapping.VendorId == parameter.VendorId && product.Active == true
                                   select product).ToList();

                return new GetProductByVendorIDResult
                {
                    lstProduct = listProduct,
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new GetProductByVendorIDResult
                {
                    Message = ex.ToString(),
                    Status = false
                };
            }
        }

        public GetProductAttributeByProductIDResult GetProductAttributeByProductID(GetProductAttributeByProductIDParameter parameter)
        {
            try
            {
                var lstProductAtribute = (from pattributecategory in context.ProductAttributeCategory
                                          join productattribute in context.ProductAttribute on pattributecategory.ProductAttributeCategoryId equals productattribute.ProductAttributeCategoryId
                                          where productattribute.ProductId == parameter.ProductId
                                          select new ProductAttributeCategory
                                          {
                                              Active = pattributecategory.Active,
                                              CreatedById = pattributecategory.CreatedById,
                                              CreatedDate = pattributecategory.CreatedDate,
                                              ProductAttributeCategoryId = pattributecategory.ProductAttributeCategoryId,
                                              ProductAttributeCategoryName = pattributecategory.ProductAttributeCategoryName,
                                              UpdatedById = pattributecategory.UpdatedById,
                                              UpdatedDate = pattributecategory.UpdatedDate,
                                              ProductAttributeCategoryValue = (context.ProductAttributeCategoryValue.Where(m => m.ProductAttributeCategoryId == pattributecategory.ProductAttributeCategoryId)).ToList(),
                                          }).ToList();

                return new GetProductAttributeByProductIDResult
                {
                    lstProductAttributeCategory = lstProductAtribute,
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new GetProductAttributeByProductIDResult
                {
                    Message = ex.ToString(),
                    Status = false
                };
            }
        }

        public GetAllProductCodeResult GetAllProductCode(GetAllProductCodeParameter parameter)
        {
            try
            {
                var ListCode = context.Product.Select(item => new { code = item.ProductCode.ToLower() }).ToList();
                List<string> result = new List<string>();
                foreach (var item in ListCode)
                {
                    result.Add(item.code.Trim());
                }
                return new GetAllProductCodeResult
                {
                    Message = "Success",
                    Status = true,
                    ListProductCode = result
                };
            }
            catch (Exception ex)
            {

                return new GetAllProductCodeResult
                {
                    Message = ex.ToString(),
                    Status = false
                };
            }
        }

        public UpdateActiveProductResult UpdateActiveProduct(UpdateActiveProductParameter parameter)
        {
            try
            {
                var productUpdate = context.Product.FirstOrDefault(item => item.ProductId == parameter.ProductId);
                productUpdate.Active = false;
                productUpdate.UpdatedById = parameter.UserId;
                productUpdate.UpdatedDate = DateTime.Now;

                context.Product.Update(productUpdate);
                context.SaveChanges();

                return new UpdateActiveProductResult
                {
                    Message = CommonMessage.ProductCategory.DELETE_SUCCESS,
                    Status = true
                };
            }
            catch (Exception ex)
            {

                return new UpdateActiveProductResult
                {
                    Message = CommonMessage.ProductCategory.DELETE_FAIL,
                    Status = false
                };
            }
        }

        public int CountProductInformation(
            Guid productId,
            List<VendorOrderDetail> vendorOrderDetails,
            //List<VendorOrderProductDetailProductAttributeValue> vendorOrderProductDetailProductAttributeValues,
            List<CustomerOrderDetail> customerOrderDetails,
            //List<OrderProductDetailProductAttributeValue> orderProductDetailProductAttributeValues,
            List<QuoteDetail> quoteDetails,
            //List<QuoteProductDetailProductAttributeValue> quoteProductDetailProductAttributeValues,
            List<ProcurementRequestItem> procurementRequestItems)
        //List<ProductAttribute> productAttributes)
        {
            int count = vendorOrderDetails.Where(q => q.ProductId == productId).Count(); //đơn đặt hàng nhà cung cấp
            //count += vendorOrderProductDetailProductAttributeValues.Where(q => q.ProductId == productId).Count();
            count += customerOrderDetails.Where(q => q.ProductId == productId).Count(); //đơn hàng
            //count += orderProductDetailProductAttributeValues.Where(q => q.ProductId == productId).Count();
            count += quoteDetails.Where(q => q.ProductId == productId).Count();//báo giá
            //count += quoteProductDetailProductAttributeValues.Where(q => q.ProductId == productId).Count();
            count += procurementRequestItems.Where(q => q.ProductId == productId).Count(); //đề xuất mua hàng   
            //count += productAttributes.Where(q => q.ProductId == productId).Count();
            //count += productVendorMappings.Where(q => q.ProductId == productId).Count(); Comment by Dung
            //tìm theo điều kiện đơn đặt hàng nhà cung cấp
            //count += vendorOrderDetail.Where(w => w.ProductId == productId).Count();
            return count;
        }

        public GetListProductResult GetListProduct(GetListProductParameter parameter)
        {
            try
            {
                var listProductCategory = new List<DataAccess.Databases.Entities.ProductCategory>();
                var listVendor = new List<DataAccess.Databases.Entities.Vendor>();

                var unitCode = "DNH"; //đơn vị tính
                var propertyCode = "TC";
                var priceInvetoryCode = "GTK";

                var unitId = context.CategoryType.Where(w => w.CategoryTypeCode == unitCode).FirstOrDefault().CategoryTypeId;
                var propertyId = context.CategoryType.Where(c => c.CategoryTypeCode == propertyCode).FirstOrDefault().CategoryTypeId;
                var priceInventoryId = context.CategoryType.Where(c => c.CategoryTypeCode == priceInvetoryCode).FirstOrDefault().CategoryTypeId;

                var listUnitEntity = context.Category.Where(w => w.CategoryTypeId == unitId).ToList();
                var listPropertyEntity = context.Category.Where(c => c.CategoryTypeId == propertyId).ToList();
                var listPriceInventoryEntity = context.Category.Where(c => c.CategoryTypeId == priceInventoryId).ToList();


                var listProductCategoryEntity = context.ProductCategory.Where(w => w.Active == true).ToList();
                var listVendorEntity = context.Vendor.Where(w => w.Active == true).ToList();


                listProductCategoryEntity?.ForEach(e =>
                {
                    listProductCategory.Add(new ProductCategory
                    {
                        ProductCategoryId = e.ProductCategoryId,
                        ProductCategoryName = e.ProductCategoryName,
                    });
                });

                listVendorEntity?.ForEach(e =>
                {
                    listVendor.Add(new Vendor
                    {
                        VendorId = e.VendorId,
                        VendorName = e.VendorName
                    });
                });

                return new GetListProductResult
                {
                    Status = true,
                    ListProductCategory = listProductCategory,
                    ListVendor = listVendor,
                    ListUnit = listUnitEntity,
                    ListPriceInventory = listPriceInventoryEntity,
                    ListProperty = listPropertyEntity
                };
            }
            catch (Exception ex)
            {

                return new GetListProductResult
                {
                    Status = false,
                    Message = ex.ToString()
                };
            }
        }

        public GetMasterdataCreateProductResult GetMasterdataCreateProduct(GetMasterdataCreateProductParameter parameter)
        {
            try
            {
                var ListProductMoneyUnit = new List<DataAccess.Databases.Entities.Category>();
                var ListProductUnit = new List<DataAccess.Databases.Entities.Category>();
                var ListVendor = new List<DataAccess.Databases.Entities.Vendor>();
                var ListWarehouse = new List<DataAccess.Databases.Entities.Warehouse>();
                var listProductCode = new List<string>();
                var listProductUnitName = new List<string>();

                #region Get data from Database
                var moneyUnitCode = "DTI"; //đơn vị tiền
                var unitCode = "DNH"; //đơn vị tính
                var propertyCode = "TC";
                var priceInvetoryCode = "GTK";

                var moneyUnitId = context.CategoryType.Where(w => w.CategoryTypeCode == moneyUnitCode).FirstOrDefault().CategoryTypeId;
                var unitId = context.CategoryType.Where(w => w.CategoryTypeCode == unitCode).FirstOrDefault().CategoryTypeId;
                var propertyId = context.CategoryType.Where(c => c.CategoryTypeCode == propertyCode).FirstOrDefault().CategoryTypeId;
                var priceInventoryId = context.CategoryType.Where(c => c.CategoryTypeCode == priceInvetoryCode).FirstOrDefault().CategoryTypeId;

                var listMoneyUnitEntity = context.Category.Where(w => w.Active == true && w.CategoryTypeId == moneyUnitId).ToList();
                var listUnitEntity = context.Category.Where(w => w.Active == true && w.CategoryTypeId == unitId).ToList();
                var listPropertyEntity = context.Category.Where(c => c.Active == true && c.CategoryTypeId == propertyId).ToList();
                var listPriceInventoryEntity = context.Category.Where(c => c.Active == true && c.CategoryTypeId == priceInventoryId).ToList();

                var warehouseEntity = context.Warehouse.Where(w => w.Active == true).ToList();
                var vendorEntity = context.Vendor.Where(w => w.Active == true).OrderBy(w => w.VendorName).ToList();

                var productCodeEntity = context.Product.Select(w => new { w.ProductCode }).ToList();

                #endregion

                #region Patch to Response
                listMoneyUnitEntity?.ForEach(e =>
                {
                    ListProductMoneyUnit.Add(new Category
                    {
                        CategoryId = e.CategoryId,
                        CategoryName = e.CategoryName,
                        CategoryCode = e.CategoryCode,
                        IsDefauld = e.IsDefauld
                    });
                });

                listUnitEntity?.ForEach(e =>
                {
                    ListProductUnit.Add(new Category
                    {
                        CategoryId = e.CategoryId,
                        CategoryName = e.CategoryName,
                        CategoryCode = e.CategoryCode,
                        IsDefauld = e.IsDefauld
                    });
                });

                warehouseEntity?.ForEach(e =>
                {
                    ListWarehouse.Add(new Warehouse
                    {
                        WarehouseId = e.WarehouseId,
                        WarehouseCode = e.WarehouseCode,
                        WarehouseName = e.WarehouseName,
                        WarehouseParent = e.WarehouseParent
                    });
                });

                ListWarehouse = ListWarehouse.OrderBy(w => w.WarehouseName).ToList();

                vendorEntity?.ForEach(e =>
                {
                    ListVendor.Add(new Vendor
                    {
                        VendorId = e.VendorId,
                        VendorName = e.VendorName,
                        VendorCode = e.VendorCode
                    });
                });

                ListVendor = ListVendor.OrderBy(w => w.VendorName).ToList();

                productCodeEntity?.ForEach(productCode =>
                {
                    listProductCode.Add(productCode.ProductCode?.Trim());
                });
                #endregion

                var categoryType = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "DNH");
                listProductUnitName = context.Category.Where(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.Active == true).Select(c => c.CategoryName).ToList();

                return new GetMasterdataCreateProductResult
                {
                    Status = true,
                    ListProductMoneyUnit = ListProductMoneyUnit,
                    ListProductUnit = ListProductUnit,
                    ListVendor = ListVendor,
                    ListWarehouse = ListWarehouse,
                    ListProductCode = listProductCode,
                    ListProductUnitName = listProductUnitName,
                    ListProperty = listPropertyEntity,
                    ListPriceInventory = listPriceInventoryEntity,
                };
            }
            catch (Exception ex)
            {

                return new GetMasterdataCreateProductResult
                {
                    Status = false,
                    Message = ex.ToString()
                };
            }
        }

        public AddSerialNumberResult AddSerialNumber(AddSerialNumberParameter parameter)
        {
            try
            {
                var listSerialEntity = context.Serial.Where(x => x.ProductId != parameter.ProductId)
                    .Select(w => new { w.SerialCode }).ToList();
                var ListSerialNumber = new List<string>();

                listSerialEntity.ForEach(serial =>
                {
                    ListSerialNumber.Add(serial.SerialCode?.Trim());
                });

                return new AddSerialNumberResult
                {
                    ListSerialNumber = ListSerialNumber,
                    Status = true
                };
            }
            catch (Exception ex)
            {

                return new AddSerialNumberResult
                {
                    Status = false,
                    Message = ex.ToString()
                };
            }
        }

        public GetMasterDataVendorDialogResult GetMasterDataVendorDialog(GetMasterDataVendorDialogParameter parameter)
        {
            var ListProductMoneyUnit = new List<DataAccess.Databases.Entities.Category>();
            var ListVendor = new List<DataAccess.Databases.Entities.Vendor>();

            var moneyUnitCode = "DTI"; //đơn vị tiền
            var moneyUnitId = context.CategoryType.Where(w => w.Active == true && w.CategoryTypeCode == moneyUnitCode).FirstOrDefault().CategoryTypeId; ;
            var listMoneyUnitEntity = context.Category.Where(w => w.Active == true && w.CategoryTypeId == moneyUnitId).ToList();
            var vendorEntity = context.Vendor.Where(w => w.Active == true).OrderBy(w => w.VendorName).ToList();
            var listProduct = context.Product.Where(w => w.Active == true).OrderBy(w => w.ProductName).ToList();
            var listSuggestedSupplierQuote = context.SuggestedSupplierQuotes.Where(c => c.Active == true).OrderBy(w => w.SuggestedSupplierQuote).ToList();

            listMoneyUnitEntity?.ForEach(e =>
            {
                ListProductMoneyUnit.Add(new Category
                {
                    CategoryId = e.CategoryId,
                    CategoryName = e.CategoryName,
                    CategoryCode = e.CategoryCode,
                    IsDefauld = e.IsDefauld
                });
            });

            vendorEntity?.ForEach(e =>
            {
                ListVendor.Add(new Vendor
                {
                    VendorId = e.VendorId,
                    VendorName = e.VendorName,
                    VendorCode = e.VendorCode
                });
            });

            ListVendor = ListVendor.OrderBy(w => w.VendorName).ToList();

            return new GetMasterDataVendorDialogResult
            {
                Status = true,
                Message = "",
                ListProductMoneyUnit = ListProductMoneyUnit,
                ListVendor = ListVendor,
                ListProduct = listProduct,
                ListSuggestedSupplierQuote = listSuggestedSupplierQuote,
            };
        }

        public DownloadTemplateProductServiceResult DownloadTemplateProductService(DownloadTemplateProductServiceParameter parameter)
        {
            try
            {
                string rootFolder = _hostingEnvironment.WebRootPath + "\\ExcelTemplate";
                string fileName = @"Template_Import_Product.xlsx";

                //FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
                string newFilePath = Path.Combine(rootFolder, fileName);
                byte[] data = File.ReadAllBytes(newFilePath);

                return new DownloadTemplateProductServiceResult
                {
                    TemplateExcel = data,
                    Message = string.Format("Đã dowload file Template_Import_Product"),
                    FileName = "Template_Import_Product",
                    Status = true
                };
            }
            catch (Exception)
            {
                return new DownloadTemplateProductServiceResult
                {
                    Message = "Đã có lỗi xảy ra trong quá trình download",
                    Status = false
                };
            }
        }

        public GetMasterDataPriceProductResult GetMasterDataPriceList(GetMasterDataPriceProductParameter parameter)
        {
            try
            {
                var groupCustomerId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "NHA").CategoryTypeId;
                var groupCustomerAll = context.Category.Where(c => c.CategoryTypeId == groupCustomerId).ToList();
                var groupCustomer = groupCustomerAll.Where(c => c.Active == true && c.CategoryTypeId == groupCustomerId).ToList();

                var listProduct = context.Product.ToList();
                var products = listProduct.Where(c => c.Active == true).ToList();

                var listPrice = context.PriceProduct.Where(c => c.Active).OrderByDescending(x => x.EffectiveDate).ToList();
                var listPriceEntityModel = new List<PriceProductEntityModel>();

                listPrice.ForEach(item =>
                {
                    var newPriceProduct = new PriceProductEntityModel
                    {
                        PriceProductId = item.PriceProductId,
                        ProductId = item.ProductId,
                        ProductCode = listProduct.FirstOrDefault(c => c.ProductId == item.ProductId)?.ProductCode ?? "",
                        ProductName = listProduct.FirstOrDefault(c => c.ProductId == item.ProductId)?.ProductName ?? "",
                        EffectiveDate = item.EffectiveDate,
                        PriceVnd = item.PriceVnd,
                        MinQuantity = item.MinQuantity,
                        PriceForeignMoney = item.PriceForeignMoney,
                        CustomerGroupCategory = item.CustomerGroupCategory,
                        CustomerGroupCategoryName = groupCustomerAll.FirstOrDefault(c => c.CategoryId == item.CustomerGroupCategory)?.CategoryName ?? "",
                        CreatedById = item.CreatedById,
                        CreatedDate = item.CreatedDate,
                    };

                    listPriceEntityModel.Add(newPriceProduct);
                });
                listPriceEntityModel = listPriceEntityModel.OrderByDescending(c => c.EffectiveDate).ToList();

                return new GetMasterDataPriceProductResult
                {
                    ListProduct = products,
                    ListPrice = listPriceEntityModel,
                    ListCategory = groupCustomer,
                    Status = true,
                    Message = "",
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataPriceProductResult
                {
                    Status = false,
                    Message = ex.Message,
                };
            }

        }

        public CreateOrUpdatePriceProductResult CreateOrUpdatePriceProduct(CreateOrUpdatePriceProductParameter parameter)
        {
            try
            {
                var priceProduct = context.PriceProduct.FirstOrDefault(c => c.PriceProductId == parameter.PriceProduct.PriceProductId);

                var groupCustomerId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "NHA").CategoryTypeId;
                var groupCustomerAll = context.Category.Where(c => c.CategoryTypeId == groupCustomerId).ToList();

                var listProduct = context.Product.ToList();
                var products = listProduct.Where(c => c.Active == true).ToList();

                if (priceProduct == null)
                {
                    var newPriceProduct = new PriceProduct
                    {
                        PriceProductId = Guid.NewGuid(),
                        ProductId = parameter.PriceProduct.ProductId,
                        EffectiveDate = parameter.PriceProduct.EffectiveDate,
                        PriceVnd = parameter.PriceProduct.PriceVnd,
                        MinQuantity = parameter.PriceProduct.MinQuantity,
                        PriceForeignMoney = parameter.PriceProduct.PriceForeignMoney,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        CustomerGroupCategory = parameter.PriceProduct.CustomerGroupCategory,
                        UpdatedById = null,
                        UpdatedDate = null
                    };
                    context.PriceProduct.Add(newPriceProduct);
                }
                else
                {
                    priceProduct.ProductId = parameter.PriceProduct.ProductId;
                    priceProduct.EffectiveDate = parameter.PriceProduct.EffectiveDate;
                    priceProduct.PriceVnd = parameter.PriceProduct.PriceVnd;
                    priceProduct.MinQuantity = parameter.PriceProduct.MinQuantity;
                    priceProduct.PriceForeignMoney = parameter.PriceProduct.PriceForeignMoney;
                    priceProduct.CustomerGroupCategory = parameter.PriceProduct.CustomerGroupCategory;
                    priceProduct.UpdatedById = parameter.UserId;
                    priceProduct.UpdatedDate = DateTime.Now;

                    context.PriceProduct.Update(priceProduct);
                }
                context.SaveChanges();

                var listPrice = context.PriceProduct.Where(c => c.Active).OrderByDescending(x => x.EffectiveDate).ToList();
                var listPriceEntityModel = new List<PriceProductEntityModel>();
                listPrice.ForEach(item =>
                {
                    var newPriceProduct = new PriceProductEntityModel
                    {
                        PriceProductId = item.PriceProductId,
                        ProductId = item.ProductId,
                        ProductCode = listProduct.FirstOrDefault(c => c.ProductId == item.ProductId)?.ProductCode ?? "",
                        ProductName = listProduct.FirstOrDefault(c => c.ProductId == item.ProductId)?.ProductName ?? "",
                        EffectiveDate = item.EffectiveDate,
                        PriceVnd = item.PriceVnd,
                        MinQuantity = item.MinQuantity,
                        PriceForeignMoney = item.PriceForeignMoney,
                        CustomerGroupCategory = item.CustomerGroupCategory,
                        CustomerGroupCategoryName = groupCustomerAll.FirstOrDefault(c => c.CategoryId == item.CustomerGroupCategory)?.CategoryName ?? "",
                        CreatedById = item.CreatedById,
                        CreatedDate = item.CreatedDate,
                    };

                    listPriceEntityModel.Add(newPriceProduct);
                });
                listPriceEntityModel = listPriceEntityModel.OrderByDescending(c => c.EffectiveDate).ToList();

                return new CreateOrUpdatePriceProductResult
                {
                    Status = true,
                    Message = Common.CommonMessage.PriceProduct.CREATE_SUCCESS,
                    ListPrice = listPriceEntityModel
                };
            }
            catch (Exception ex)
            {
                return new CreateOrUpdatePriceProductResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public DeletePriceProductResult DeletePriceProduct(DeletePriceProductParameter parameter)
        {
            try
            {
                var priceProduct = context.PriceProduct.FirstOrDefault(c => c.PriceProductId == parameter.PriceProductId);
                if (priceProduct == null)
                {
                    return new DeletePriceProductResult
                    {
                        Status = false,
                        Message = CommonMessage.PriceProduct.DELETE_FAIL
                    };
                }
                else
                {
                    priceProduct.Active = false;
                    context.PriceProduct.Update(priceProduct);
                    context.SaveChanges();
                    return new DeletePriceProductResult
                    {
                        Status = true,
                        Message = CommonMessage.PriceProduct.DELETE_SUCCESS
                    };
                }
            }
            catch (Exception ex)
            {
                return new DeletePriceProductResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public GetDataQuickCreateProductResult GetDataQuickCreateProduct(GetDataQuickCreateProductParameter parameter)
        {
            try
            {
                //var listProductCode = new List<string>();
                //var ListProductUnit = new List<DataAccess.Databases.Entities.Category>();

                //var unitCode = "DNH"; //đơn vị tính
                //var priceInvetoryCode = "GTK";
                //var propertyCode = "TC";

                //var unitId = context.CategoryType.Where(w => w.CategoryTypeCode == unitCode).FirstOrDefault().CategoryTypeId;

                //var priceInventoryId = context.CategoryType.Where(c => c.CategoryTypeCode == priceInvetoryCode).FirstOrDefault().CategoryTypeId;

                //var productCodeEntity = context.Product.Select(w => new { w.ProductCode }).ToList();
                //var listUnitEntity = context.Category.Where(w => w.Active == true && w.CategoryTypeId == unitId).ToList();


                //productCodeEntity?.ForEach(productCode =>
                //{
                //    listProductCode.Add(productCode.ProductCode?.Trim());
                //});

                //listUnitEntity?.ForEach(e =>
                //{
                //    ListProductUnit.Add(new Category
                //    {
                //        CategoryId = e.CategoryId,
                //        CategoryName = e.CategoryName,
                //        CategoryCode = e.CategoryCode,
                //        IsDefauld = e.IsDefauld
                //    });
                //});

                //var propertyId = context.CategoryType.Where(c => c.CategoryTypeCode == propertyCode).FirstOrDefault().CategoryTypeId;
                //var listPriceInventoryEntity = context.Category.Where(c => c.Active == true && c.CategoryTypeId == priceInventoryId).ToList();
                //var listPropertyEntity = context.Category.Where(c => c.Active == true && c.CategoryTypeId == propertyId).ToList();

                #region Mã sản phẩm
                var listProductCode = context.Product.Where(w => w.Active == true).Select(w => w.ProductCode).ToList() ?? new List<string>();
                #endregion

                #region Đơn vị tính
                var listProductUnit = new List<DataAccess.Models.CategoryEntityModel>();

                var unitCode = "DNH"; //đơn vị tính
                var unitId = context.CategoryType.Where(w => w.CategoryTypeCode == unitCode).FirstOrDefault().CategoryTypeId;
                var listUnitEntity = context.Category.Where(w => w.Active == true && w.CategoryTypeId == unitId).ToList();
                listUnitEntity?.ForEach(e =>
                {
                    listProductUnit.Add(new DataAccess.Models.CategoryEntityModel
                    {
                        CategoryId = e.CategoryId,
                        CategoryName = e.CategoryName,
                        CategoryCode = e.CategoryCode,
                    });
                });
                #endregion

                #region Cách tính tồn kho
                var listPriceInventory = new List<DataAccess.Models.CategoryEntityModel>();

                var priceInvetoryCode = "GTK";
                var priceInventoryId = context.CategoryType.Where(c => c.CategoryTypeCode == priceInvetoryCode).FirstOrDefault().CategoryTypeId;
                var listPriceInventoryEntity = context.Category.Where(c => c.Active == true && c.CategoryTypeId == priceInventoryId).ToList();
                listPriceInventoryEntity?.ForEach(e =>
                {
                    listPriceInventory.Add(new DataAccess.Models.CategoryEntityModel
                    {
                        CategoryId = e.CategoryId,
                        CategoryName = e.CategoryName,
                        CategoryCode = e.CategoryCode,
                    });
                });
                #endregion

                #region Tính chất
                var listProperty = new List<DataAccess.Models.CategoryEntityModel>();

                var propertyCode = "TC";
                var propertyId = context.CategoryType.Where(c => c.CategoryTypeCode == propertyCode).FirstOrDefault().CategoryTypeId;
                var listPropertyEntity = context.Category.Where(c => c.Active == true && c.CategoryTypeId == propertyId).ToList();
                listPropertyEntity?.ForEach(e =>
                {
                    listProperty.Add(new DataAccess.Models.CategoryEntityModel
                    {
                        CategoryId = e.CategoryId,
                        CategoryName = e.CategoryName,
                        CategoryCode = e.CategoryCode,
                    });
                });
                #endregion

                return new GetDataQuickCreateProductResult
                {
                    ListProductCode = listProductCode,
                    ListProductUnit = listProductUnit,
                    ListPriceInventory = listPriceInventory,
                    ListProperty = listProperty,
                    Status = true,
                    Message = ""
                };
            }
            catch (Exception ex)
            {
                return new GetDataQuickCreateProductResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public GetDataCreateUpdateBOMResult GetDataCreateUpdateBOM(GetDataCreateUpdateBOMParameter parameter)
        {
            try
            {
                #region Lấy danh sách sản phẩm
                var listProductEntity = context.Product.Where(w => w.Active == true).ToList();
                var unitTypeCodeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "DNH").CategoryTypeId;
                var listProductUnitEntity = context.Category.Where(w => w.CategoryTypeId == unitTypeCodeId && w.Active == true).ToList();

                var listProduct = new List<DataAccess.Models.Product.ProductEntityModel>();

                listProductEntity?.ForEach(product =>
                {
                    var productUnitName = listProductUnitEntity.FirstOrDefault(f => f.CategoryId == product.ProductUnitId)?.CategoryName ?? "";

                    listProduct.Add(new ProductEntityModel()
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        ProductCode = product.ProductCode,
                        ProductUnitId = product.ProductUnitId,
                        ProductUnitName = productUnitName
                    });
                });
                #endregion

                return new GetDataCreateUpdateBOMResult
                {
                    ListProduct = listProduct,
                    Status = true,
                    Message = ""
                };
            }
            catch (Exception ex)
            {
                return new GetDataCreateUpdateBOMResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public DownloadPriceProductTemplateResult DownloadPriceProductTemplate(DownloadPriceProductTemplateParameter parameter)
        {
            try
            {
                string rootFolder = _hostingEnvironment.WebRootPath + "\\ExcelTemplate";
                string fileName = @"Template_import_Bng_gi_bn.xlsx";

                //FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
                string newFilePath = Path.Combine(rootFolder, fileName);
                byte[] data = File.ReadAllBytes(newFilePath);

                return new DownloadPriceProductTemplateResult
                {
                    TemplateExcel = data,
                    Message = string.Format("Đã dowload file Template_import_Bng_gi_bn"),
                    FileName = "Template_import_Bng_gi_bn",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new DownloadPriceProductTemplateResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public ImportPriceProductResult ImportPriceProduct(ImportPriceProductParamter parameter)
        {
            try
            {
                if (parameter.ListPriceProduct == null)
                {
                    return new ImportPriceProductResult
                    {
                        Message = "not foud",
                        Status = false
                    };
                }
                var list = new List<PriceProduct>();
                parameter.ListPriceProduct.ForEach(item =>
                {
                    var priceProduct = new PriceProduct
                    {
                        PriceProductId = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        EffectiveDate = item.EffectiveDate,
                        MinQuantity = item.MinQuantity,
                        PriceVnd = item.PriceVnd,
                        PriceForeignMoney = item.PriceForeignMoney,
                        CustomerGroupCategory = item.CustomerGroupCategory,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        UpdatedById = null,
                        UpdatedDate = null
                    };
                    list.Add(priceProduct);
                });
                context.PriceProduct.AddRange(list);
                context.SaveChanges();

                return new ImportPriceProductResult
                {
                    Message = "Success",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ImportPriceProductResult
                {
                    Message = ex.Message,
                    Status = false
                };
            }
        }
    }
}
