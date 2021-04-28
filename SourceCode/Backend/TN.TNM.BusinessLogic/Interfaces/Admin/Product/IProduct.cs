using TN.TNM.BusinessLogic.Messages.Requests.Admin.Product;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Product;

namespace TN.TNM.BusinessLogic.Interfaces.Admin.Product
{
    public interface IProduct
    {
        SearchProductResponse SearchProduct(SearchProductRequest parameter);
        CreateProductResponse CreateProduct(CreateProductRequest request);
        GetProductByIDResponse GetProductByID(GetProductByIDRequest request);
        UpdateActiveProductResponse UpdateActiveProduct(UpdateActiveProductRequest request);
        UpdateProductResponse UpdateProduct(UpdateProductRequest request);
        GetProductByVendorIDResponse GetProductByVendorID(GetProductByVendorIDRequest request);
        GetProductAttributeByProductIDResponse GetProductAttributeByProductID(GetProductAttributeByProductIDRequest request);
        GetAllProductCodeResponse GetAllProductCode(GetAllProductCodeRequest request);
        GetListProductResponse GetListProduct(GetListProductRequest request);
        GetMasterdataCreateProductResponse GetMasterdataCreateProduct(GetMasterdataCreateProductRequest request);
        AddSerialNumberResponse AddSerialNumber(AddSerialNumberRequest request);
        GetMasterDataVendorDialogResponse GetMasterDataVendorDialog(GetMasterDataVendorDialogRequest request);
        DownloadTemplateProductServiceResponse DownloadTemplateProductService(DownloadTemplateProductServiceRequest request);
        ImportProductResponse ImportProduct(ImportProductRequest request);
        GetMasterDataPriceProductResponse GetMasterDataPriceList(GetMasterDataPriceProductRequest request);
        CreateOrUpdatePriceProductResponse CreateOrUpdatePriceProduct(CreateOrUpdatePriceProductRequest request);
        DeletePriceProductResponse DeletePriceProduct(DeletePriceProductRequest request);
        GetDataQuickCreateProductResponse GetDataQuickCreateProduct(GetDataQuickCreateProductRequest request);
        GetDataCreateUpdateBOMResponse GetDataCreateUpdateBOM(GetDataCreateUpdateBOMRequest request);
        DownloadPriceProductTemplateResponse DownloadPriceProductTemplate(DownloadPriceProductTemplateRequest request);
        ImportPriceProductResponse ImportPriceProduct(ImportPriceProductRequest request);
    }
}
