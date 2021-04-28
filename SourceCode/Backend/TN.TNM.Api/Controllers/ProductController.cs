using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.Product;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Product;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Product;
using Microsoft.AspNetCore.Http;


namespace TN.TNM.Api.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProduct iProduct;
        public ProductController(IProduct _iProduct)
        {
            this.iProduct = _iProduct;
        }
        /// <summary>
        /// search product
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/searchProduct")]
        [Authorize(Policy = "Member")]
        public SearchProductResponse  SearchProduct([FromBody]SearchProductRequest request)
        {
            return this.iProduct.SearchProduct(request);
        }
        /// <summary>
        /// create product
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/createProduct")]
        [Authorize(Policy = "Member")]
        public CreateProductResponse CreateProduct([FromBody]CreateProductRequest request)
        {
            return this.iProduct.CreateProduct(request);
        }
        /// <summary>
        /// GetProductByID product
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/GetProductByID")]
        [Authorize(Policy = "Member")]
        public GetProductByIDResponse GetProductByID([FromBody]GetProductByIDRequest request)
        {
            return this.iProduct.GetProductByID(request);
        }

        /// <summary>
        /// UpdateProduct
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/UpdateProduct")]
        [Authorize(Policy = "Member")]
        public UpdateProductResponse UpdateProduct([FromBody]UpdateProductRequest request)
        {
            return this.iProduct.UpdateProduct(request);
        }

        /// <summary>
        /// UpdateActiveProduct
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/updateActiveProduct")]
        [Authorize(Policy = "Member")]
        public UpdateActiveProductResponse UpdateActiveProduct([FromBody]UpdateActiveProductRequest request)
        {
            return this.iProduct.UpdateActiveProduct(request);
        }

        /// <summary>
        /// GetProductByVendorID
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/getProductByVendorID")]
        [Authorize(Policy = "Member")]
        public GetProductByVendorIDResponse GetProductByVendorID([FromBody]GetProductByVendorIDRequest request)
        {
            return this.iProduct.GetProductByVendorID(request);
        }

        /// <summary>
        /// GetProductAttributeByProductID
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/getProductAttributeByProductID")]
        [Authorize(Policy = "Member")]
        public GetProductAttributeByProductIDResponse GetProductAttributeByProductID([FromBody]GetProductAttributeByProductIDRequest request)
        {
            return this.iProduct.GetProductAttributeByProductID(request);
        }

        /// <summary>
        /// Get All Product Code
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/getAllProductCode")]
        [Authorize(Policy = "Member")]
        public GetAllProductCodeResponse GetAllProductCode([FromBody]GetAllProductCodeRequest request)
        {
            return this.iProduct.GetAllProductCode(request);
        }

        /// <summary>
        /// Get List Product
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/getListProduct")]
        [Authorize(Policy = "Member")]
        public GetListProductResponse GetListProduct([FromBody]GetListProductRequest request)
        {
            return this.iProduct.GetListProduct(request);
        }

        /// <summary>
        /// Get Masterdata Create Product
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/getMasterdataCreateProduct")]
        [Authorize(Policy = "Member")]
        public GetMasterdataCreateProductResponse GetMasterdataCreateProduct([FromBody]GetMasterdataCreateProductRequest request)
        {
            return this.iProduct.GetMasterdataCreateProduct(request);
        }

        /// <summary>
        /// Add Serial Number
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/dddSerialNumber")]
        [Authorize(Policy = "Member")]
        public AddSerialNumberResponse AddSerialNumber([FromBody]AddSerialNumberRequest request)
        {
            return this.iProduct.AddSerialNumber(request);
        }

        /// <summary>
        /// Get master data vendor dialog
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/getMasterDataVendorDialog")]
        [Authorize(Policy = "Member")]
        public GetMasterDataVendorDialogResponse GetMasterDataVendorDialog([FromBody]GetMasterDataVendorDialogRequest request)
        {
            return this.iProduct.GetMasterDataVendorDialog(request);
        }

        /// <summary>
        /// Get master data vendor dialog
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/downloadTemplateProductService")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateProductServiceResponse DownloadTemplateProductService([FromBody]DownloadTemplateProductServiceRequest request)
        {
            return this.iProduct.DownloadTemplateProductService(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/importProduct")]
        [Authorize(Policy = "Member")]
        public ImportProductResponse ImportProduct([FromBody]ImportProductRequest request)
        {
            return this.iProduct.ImportProduct(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/getMasterDataPriceList")]
        [Authorize(Policy = "Member")]
        public GetMasterDataPriceProductResponse GetMasterDataPriceList([FromBody]GetMasterDataPriceProductRequest request)
        {
            return this.iProduct.GetMasterDataPriceList(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/createOrUpdatePriceProduct")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdatePriceProductResponse CreateOrUpdatePriceProduct([FromBody]CreateOrUpdatePriceProductRequest request)
        {
            return this.iProduct.CreateOrUpdatePriceProduct(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Product/deletePriceProduct")]
        [Authorize(Policy = "Member")]
        public DeletePriceProductResponse DeletePriceProduct([FromBody]DeletePriceProductRequest request)
        {
            return this.iProduct.DeletePriceProduct(request);
        }

        [HttpPost]
        [Route("api/Product/getDataQuickCreateProduct")]
        [Authorize(Policy = "Member")]
        public GetDataQuickCreateProductResponse GetDataQuickCreateProduct([FromBody]GetDataQuickCreateProductRequest request)
        {
            return this.iProduct.GetDataQuickCreateProduct(request);
        }

        [HttpPost]
        [Route("api/Product/getDataCreateUpdateBOM")]
        [Authorize(Policy = "Member")]
        public GetDataCreateUpdateBOMResponse GetDataCreateUpdateBOM([FromBody]GetDataCreateUpdateBOMRequest request)
        {
            return this.iProduct.GetDataCreateUpdateBOM(request);
        }


        [HttpPost]
        [Route("api/Product/downloadPriceProductTemplate")]
        [Authorize(Policy = "Member")]
        public DownloadPriceProductTemplateResponse DownloadPriceProductTemplate([FromBody] DownloadPriceProductTemplateRequest request)
        {
            return this.iProduct.DownloadPriceProductTemplate(request);
        }

        [HttpPost]
        [Route("api/Product/importPriceProduct")]
        [Authorize(Policy = "Member")]
        public ImportPriceProductResponse ImportPriceProduct([FromBody] ImportPriceProductRequest request)
        {
            return this.iProduct.ImportPriceProduct(request);
        }
    }
}