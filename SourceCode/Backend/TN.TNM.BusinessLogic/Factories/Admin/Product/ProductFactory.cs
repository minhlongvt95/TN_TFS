using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Admin.Product;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Product;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Product;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.BusinessLogic.Models.ProductAttributeCategoryValue;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Admin.Product
{
    public class ProductFactory : BaseFactory, IProduct
    {
        private IProductDataAccess iProductDataAccess;
        public ProductFactory(IProductDataAccess _iProductDataAccess, ILogger<ProductFactory> _logger)
        {
            this.iProductDataAccess = _iProductDataAccess;
            this.logger = _logger;
        }

        public CreateProductResponse CreateProduct(CreateProductRequest request)
        {
            try
            {
                this.logger.LogInformation("Create Product and Service");
                var parameter = request.ToParameter();
                var result = iProductDataAccess.CreateProduct(parameter);
                var response = new CreateProductResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ProductId = result.Status ? result.ProductId : Guid.Empty,
                    NewProduct = result.NewProduct
                };
                return response;
            }
            catch (Exception ex)
            {
                var response = new CreateProductResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString(),
                };
                return response;
            }
        }

        public ImportProductResponse ImportProduct(ImportProductRequest request)
        {
            try
            {
                this.logger.LogInformation("Import product");
                var paramter = request.ToParameter();
                var result = iProductDataAccess.ImportProduct(paramter);
                var response = new ImportProductResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception ex)
            {
                return new ImportProductResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message,
                };
            }
        }

        public GetAllProductCodeResponse GetAllProductCode(GetAllProductCodeRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All Product Code");
                var parameter = request.ToParameter();
                var result = iProductDataAccess.GetAllProductCode(parameter);
                var response = new GetAllProductCodeResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    ListProductCode = result.ListProductCode
                };
                return response;
            }
            catch (Exception ex)
            {
                return new GetAllProductCodeResponse
                {
                    MessageCode = ex.ToString(),
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };

            }
        }

        public GetProductAttributeByProductIDResponse GetProductAttributeByProductID(GetProductAttributeByProductIDRequest request)
        {
            try
            {
                this.logger.LogInformation("GetGetProductAttributeByProductID");
                var parameter = request.ToParameter();
                var result = iProductDataAccess.GetProductAttributeByProductID(parameter);
                var response = new GetProductAttributeByProductIDResponse
                {
                    lstProductAttributeCategory = new List<Models.ProductAttributeCategory.ProductAttributeCategoryModel>(),
                    MessageCode = result.Message,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
                result.lstProductAttributeCategory.ForEach(item =>
                {
                    List<ProductAttributeCategoryValueModel> lstProductAttributeCategoryValueModel = new List<ProductAttributeCategoryValueModel>();
                    foreach (var itemProductAttributeCategoryValue in item.ProductAttributeCategoryValue)
                    {
                        lstProductAttributeCategoryValueModel.Add(new ProductAttributeCategoryValueModel(itemProductAttributeCategoryValue));
                    }
                    var itemPush = new Models.ProductAttributeCategory.ProductAttributeCategoryModel
                    {
                        Active = item.Active,
                        CreatedById = item.CreatedById,
                        CreatedDate = item.CreatedDate,
                        ProductAttributeCategoryId = item.ProductAttributeCategoryId,
                        ProductAttributeCategoryName = item.ProductAttributeCategoryName,
                        UpdatedById = item.UpdatedById,
                        UpdatedDate = item.UpdatedDate,
                        ProductAttributeCategoryValue = lstProductAttributeCategoryValueModel
                    };

                    response.lstProductAttributeCategory.Add(itemPush);
                });
                return response;

            }
            catch (Exception ex)
            {
                return new GetProductAttributeByProductIDResponse
                {
                    MessageCode = ex.ToString(),
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetProductByIDResponse GetProductByID(GetProductByIDRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Product by ID");
                var parameter = request.ToParameter();
                var result = iProductDataAccess.GetProductByID(parameter);
                var response = new GetProductByIDResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    Product = new Models.Product.ProductModel(result.Product),
                    LstProductVendorMapping = new List<Models.Product.ProductVendorMappingModel>(),
                    lstProductAttributeCategory = new List<Models.ProductAttributeCategory.ProductAttributeCategoryModel>(),
                    lstCustomerOrder = new List<CustomerOrderModel>(),
                    ListInventory = result.ListInventory,
                    ListProductImage = result.ListProductImage,
                    ListProductBillOfMaterials = result.ListProductBillOfMaterials,
                    CanDelete = result.CanDelete
                    //CountProductInformation = result.CountProductInformation,
                };
                result.lstProductAttributeCategory.ForEach(item =>
                {
                    List<ProductAttributeCategoryValueModel> lstProductAttributeCategoryValueModel = new List<ProductAttributeCategoryValueModel>();
                    foreach (var itemProductAttributeCategoryValue in item.ProductAttributeCategoryValue)
                    {
                        lstProductAttributeCategoryValueModel.Add(new ProductAttributeCategoryValueModel(itemProductAttributeCategoryValue));
                    }
                    var itemPush = new Models.ProductAttributeCategory.ProductAttributeCategoryModel
                    {
                        Active = item.Active,
                        CreatedById = item.CreatedById,
                        CreatedDate = item.CreatedDate,
                        ProductAttributeCategoryId = item.ProductAttributeCategoryId,
                        ProductAttributeCategoryName = item.ProductAttributeCategoryName,
                        UpdatedById = item.UpdatedById,
                        UpdatedDate = item.UpdatedDate,
                        ProductAttributeCategoryValue = lstProductAttributeCategoryValueModel
                    };

                    response.lstProductAttributeCategory.Add(itemPush);
                });

                result.LstProductVendorMapping.ForEach(item =>
                {
                    response.LstProductVendorMapping.Add(new Models.Product.ProductVendorMappingModel(item));
                });

                if (result.lstCustomerOrder.Count > 0)
                {
                    result.lstCustomerOrder.ForEach(item =>
                    {
                        response.lstCustomerOrder.Add(new CustomerOrderModel(item));
                    });
                }
                return response;
            }
            catch (Exception ex)
            {
                return new GetProductByIDResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public GetProductByVendorIDResponse GetProductByVendorID(GetProductByVendorIDRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Product by VendorID");
                var parameter = request.ToParameter();
                var result = iProductDataAccess.GetGetProductByVendorID(parameter);
                var response = new GetProductByVendorIDResponse
                {
                    lstProduct = new List<Models.Product.ProductModel>(),
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                };
                result.lstProduct.ForEach(item => { response.lstProduct.Add(new Models.Product.ProductModel(item)); });
                return response;
            }
            catch (Exception ex)
            {
                return new GetProductByVendorIDResponse
                {
                    MessageCode = ex.ToString(),
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public SearchProductResponse SearchProduct(SearchProductRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Product and Services");
                var parameter = request.ToParameter();
                var result = iProductDataAccess.SearchProduct(parameter);
                var response = new SearchProductResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    ProductList = result.ProductList
                };
                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogInformation(ex.Message);
                var response = new SearchProductResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
                return response;
            }
        }

        public UpdateActiveProductResponse UpdateActiveProduct(UpdateActiveProductRequest request)
        {
            try
            {
                this.logger.LogInformation("Update Active Product");
                var parameter = request.ToParameter();
                var result = iProductDataAccess.UpdateActiveProduct(parameter);
                var response = new UpdateActiveProductResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception ex)
            {
                return new UpdateActiveProductResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public UpdateProductResponse UpdateProduct(UpdateProductRequest request)
        {
            try
            {
                this.logger.LogInformation("Update Product");
                var parameter = request.ToParameter();
                var result = iProductDataAccess.UpdateProduct(parameter);
                var response = new UpdateProductResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ProductId = result.Status ? result.ProductId : Guid.Empty
                };
                return response;
            }
            catch (Exception ex)
            {
                return new UpdateProductResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.ProductCategory.DELETE_FAIL
                };
            }
        }

        public GetListProductResponse GetListProduct(GetListProductRequest request)
        {
            try
            {
                this.logger.LogInformation("Get List Product");
                var parameter = request.ToParameter();
                var result = iProductDataAccess.GetListProduct(parameter);
                var response = new GetListProductResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListProductCategory = new List<Models.Admin.ProductCategoryModel>(),
                    ListVendor = new List<Models.Vendor.VendorModel>(),
                    ListUnit = new List<Models.Category.CategoryModel>(),
                    ListPriceInventory = new List<Models.Category.CategoryModel>(),
                    ListProperty = new List<Models.Category.CategoryModel>(),
                };
                result.ListProductCategory.ForEach(e => response.ListProductCategory.Add(new Models.Admin.ProductCategoryModel(e)));
                result.ListVendor.ForEach(e => response.ListVendor.Add(new Models.Vendor.VendorModel(e)));
                result.ListUnit.ForEach(item =>
                {
                    response.ListUnit.Add(new Models.Category.CategoryModel(item));
                });
                result.ListPriceInventory.ForEach(item =>
                {
                    response.ListPriceInventory.Add(new Models.Category.CategoryModel(item));
                });
                result.ListProperty.ForEach(item =>
                {
                    response.ListProperty.Add(new Models.Category.CategoryModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                return new GetListProductResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public GetMasterdataCreateProductResponse GetMasterdataCreateProduct(GetMasterdataCreateProductRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Master data Create Product");
                var parameter = request.ToParameter();
                var result = iProductDataAccess.GetMasterdataCreateProduct(parameter);
                var response = new GetMasterdataCreateProductResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListProductMoneyUnit = new List<Models.Category.CategoryModel>(),
                    ListProductUnit = new List<Models.Category.CategoryModel>(),
                    ListPriceInventory = new List<Models.Category.CategoryModel>(),
                    ListProperty = new List<Models.Category.CategoryModel>(),
                    ListVendor = new List<Models.Vendor.VendorModel>(),
                    ListWarehouse = new List<Models.WareHouse.WareHouseModel>(),
                    ListProductCode = new List<string>(),
                    ListProductUnitName = new List<string>()
                };
                result.ListProductMoneyUnit.ForEach(e => response.ListProductMoneyUnit.Add(new Models.Category.CategoryModel(e)));
                result.ListProductUnit.ForEach(e => response.ListProductUnit.Add(new Models.Category.CategoryModel(e)));
                result.ListVendor.ForEach(e => response.ListVendor.Add(new Models.Vendor.VendorModel(e)));
                result.ListWarehouse.ForEach(e => response.ListWarehouse.Add(new Models.WareHouse.WareHouseModel(e)));
                result.ListProductCode.ForEach(e => response.ListProductCode.Add(e));
                result.ListProductUnitName.ForEach(e => response.ListProductUnitName.Add(e));
                result.ListProperty.ForEach(e => response.ListProperty.Add(new Models.Category.CategoryModel(e)));
                result.ListPriceInventory.ForEach(e => response.ListPriceInventory.Add(new Models.Category.CategoryModel(e)));
                return response;
            }
            catch (Exception ex)
            {
                return new GetMasterdataCreateProductResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public AddSerialNumberResponse AddSerialNumber(AddSerialNumberRequest request)
        {
            try
            {
                this.logger.LogInformation("Add Serial Number");
                var parameter = request.ToParameter();
                var result = iProductDataAccess.AddSerialNumber(parameter);
                var response = new AddSerialNumberResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListSerialNumber = new List<string>()
                };
                result.ListSerialNumber.ForEach(serial => response.ListSerialNumber.Add(serial));
                return response;
            }
            catch (Exception ex)
            {
                return new AddSerialNumberResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public GetMasterDataVendorDialogResponse GetMasterDataVendorDialog(GetMasterDataVendorDialogRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProductDataAccess.GetMasterDataVendorDialog(parameter);

                var response = new GetMasterDataVendorDialogResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListProductMoneyUnit = new List<Models.Category.CategoryModel>(),
                    ListVendor = new List<Models.Vendor.VendorModel>(),
                    ListProduct = new List<Models.Product.ProductModel>(),
                    ListSuggestedSupplierQuote = new List<Models.Vendor.SuggestedSupplierQuotesModel>(),
                };
                result.ListVendor.ForEach(item =>
                {
                    response.ListVendor.Add(new Models.Vendor.VendorModel(item));
                });
                result.ListProductMoneyUnit.ForEach(item =>
                {
                    response.ListProductMoneyUnit.Add(new Models.Category.CategoryModel(item));
                });
                result.ListProduct.ForEach(item =>
                {
                    response.ListProduct.Add(new Models.Product.ProductModel(item));
                });
                result.ListSuggestedSupplierQuote.ForEach(item =>
                {
                    response.ListSuggestedSupplierQuote.Add(new Models.Vendor.SuggestedSupplierQuotesModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                return new GetMasterDataVendorDialogResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message
                };
            }
        }

        public DownloadTemplateProductServiceResponse DownloadTemplateProductService(DownloadTemplateProductServiceRequest request)
        {
            try
            {
                this.logger.LogInformation("Download Template Import Product");
                var parameter = request.ToParameter();
                var result = iProductDataAccess.DownloadTemplateProductService(parameter);

                var response = new DownloadTemplateProductServiceResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    FileName = result.FileName,
                    TemplateExcel = result.TemplateExcel
                };

                return response;
            }
            catch (Exception e)
            {
                return new DownloadTemplateProductServiceResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataPriceProductResponse GetMasterDataPriceList(GetMasterDataPriceProductRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProductDataAccess.GetMasterDataPriceList(parameter);
                var response = new GetMasterDataPriceProductResponse
                {
                    ListProduct = new List<Models.Product.ProductModel>(),
                    ListCategory = new List<Models.Category.CategoryModel>(),
                    ListPrice = new List<Models.Product.PriceProductModel>(),
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                result.ListProduct.ForEach(item =>
                {
                    response.ListProduct.Add(new Models.Product.ProductModel(item));
                });
                result.ListCategory.ForEach(item =>
                {
                    response.ListCategory.Add(new Models.Category.CategoryModel(item));
                });
                result.ListPrice.ForEach(item =>
                {
                    response.ListPrice.Add(new Models.Product.PriceProductModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                return new GetMasterDataPriceProductResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public CreateOrUpdatePriceProductResponse CreateOrUpdatePriceProduct(CreateOrUpdatePriceProductRequest request)
        {
            try
            {
                this.logger.LogInformation("Create price Product");
                var parameter = request.ToParameter();
                var result = iProductDataAccess.CreateOrUpdatePriceProduct(parameter);
                var response = new CreateOrUpdatePriceProductResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListPrice = new List<Models.Product.PriceProductModel>()
                };

                result.ListPrice.ForEach(item =>
                {
                    response.ListPrice.Add(new Models.Product.PriceProductModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                return new CreateOrUpdatePriceProductResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public DeletePriceProductResponse DeletePriceProduct(DeletePriceProductRequest request)
        {
            try
            {
                this.logger.LogInformation("Delete price Product");
                var parameter = request.ToParameter();
                var result = iProductDataAccess.DeletePriceProduct(parameter);
                return new DeletePriceProductResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
            }
            catch (Exception ex)
            {
                return new DeletePriceProductResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetDataQuickCreateProductResponse GetDataQuickCreateProduct(GetDataQuickCreateProductRequest request)
        {
            try
            {
                this.logger.LogInformation("GetDataQuickCreateProduct");
                var parameter = request.ToParameter();
                var result = iProductDataAccess.GetDataQuickCreateProduct(parameter);
                return new GetDataQuickCreateProductResponse
                {
                    ListPriceInventory = result.ListPriceInventory,
                    ListProductCode = result.ListProductCode,
                    ListProductUnit = result.ListProductUnit,
                    ListProperty = result.ListProperty,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
            }
            catch (Exception ex)
            {
                return new GetDataQuickCreateProductResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetDataCreateUpdateBOMResponse GetDataCreateUpdateBOM(GetDataCreateUpdateBOMRequest request)
        {
            try
            {
                this.logger.LogInformation("GetDataCreateUpdateBOM");
                var parameter = request.ToParameter();
                var result = iProductDataAccess.GetDataCreateUpdateBOM(parameter);
                return new GetDataCreateUpdateBOMResponse
                {
                    ListProduct = result.ListProduct,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
            }
            catch (Exception ex)
            {
                return new GetDataCreateUpdateBOMResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public DownloadPriceProductTemplateResponse DownloadPriceProductTemplate(DownloadPriceProductTemplateRequest request)
        {
            try
            {
                this.logger.LogInformation("Download Template Import Bảng giá");
                var parameter = request.ToParameter();
                var result = iProductDataAccess.DownloadPriceProductTemplate(parameter);

                var response = new DownloadPriceProductTemplateResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    FileName = result.FileName,
                    TemplateExcel = result.TemplateExcel
                };

                return response;
            }
            catch (Exception ex)
            {
                return new DownloadPriceProductTemplateResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public ImportPriceProductResponse ImportPriceProduct(ImportPriceProductRequest request)
        {
            try
            {
                var paramter = request.ToParameter();
                var result = iProductDataAccess.ImportPriceProduct(paramter);

                var response = new ImportPriceProductResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception ex)
            {
                return new ImportPriceProductResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }
    }
}
