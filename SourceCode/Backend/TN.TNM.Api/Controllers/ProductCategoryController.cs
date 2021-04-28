using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.ProductCategory;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.ProductCategory;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.ProductCategory;

namespace TN.TNM.Api.Controllers
{
    public class ProductCategoryController : Controller
    {
        private readonly IProductCategory iProductCategory;
        public ProductCategoryController(IProductCategory _iProductCategory)
        {
            this.iProductCategory = _iProductCategory;
        }
        /// <summary>
        /// Create new ProductCategory
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ProductCategory/createProductCategory")]
        [Authorize(Policy = "Member")]
        public CreateProductCategoryResponse CreateProductCategory([FromBody]CreateProductCategoryRequest request)
        {
            return this.iProductCategory.CreateProductCategory(request);
        }
        /// <summary>
        /// Get all ProductCategory
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/productCategory/getAllProductCategory")]
        [Authorize(Policy = "Member")]
        public GetAllProductCategoryResponse GetAllProductCategory([FromBody]GetAllProductCategoryRequest request)
        {
            return this.iProductCategory.GetAllProductCategory(request);
        }
        /// <summary>
        /// Get ProductCategoryById
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/productCategory/getProductCategoryById")]
        [Authorize(Policy = "Member")]
        public GetProductCategoryByIdResponse GetProductCategoryById([FromBody]GetProductCategoryByIdRequest request)
        {
            return this.iProductCategory.GetProductCategoryById(request);
        }
        /// <summary>
        /// Edit ProductCategory
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/productCategory/editProductCategory")]
        [Authorize(Policy = "Member")]
        public EditProductCategoryResponse EditProductCategory([FromBody]EditProductCategoryRequest request)
        {
            return this.iProductCategory.EditProductCategory(request);
        }
        /// <summary>
        /// Delete ProductCategory
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/productCategory/deleteProductCategory")] 
        [Authorize(Policy = "Member")]
        public DeleteProductCategoryResponse DeleteProductCategory([FromBody]DeleteProductCategoryRequest request)
        {
            return this.iProductCategory.DeleteProductCategory(request);
        }
        /// <summary>
        /// Search ProductCategory
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/productCategory/searchProductCategory")]
        [Authorize(Policy = "Member")]
        public SearchProductCategoryResponse SearchProductCategory([FromBody]SearchProductCategoryRequest request)
        {
            return this.iProductCategory.SearchProductCategory(request);
        }
        /// <summary>
        /// GetNameTreeProductCategory
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/productCategory/getNameTreeProductCategory")]
        [Authorize(Policy = "Member")]
        public GetNameTreeProductCategoryResponse GetNameTreeProductCategory([FromBody]GetNameTreeProductCategoryRequest request)
        {
            return this.iProductCategory.GetNameTreeProductCategory(request);
        }
        /// <summary>
        /// Get All Category Code
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/productCategory/getAllCategoryCode")]
        [Authorize(Policy = "Member")]
        public GetAllCategoryCodeResponse GetAllCategoryCode([FromBody]GetAllCategoryCodeRequest request)
        {
            return this.iProductCategory.GetAllCategoryCode(request);
        }

        /// <summary>
        /// Update Active Product Category
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/productCategory/updateActiveProductCategory")]
        [Authorize(Policy = "Member")]
        public UpdateActiveProductCategoryResponse UpdateActiveProductCategory([FromBody]UpdateActiveProductCategoryRequest request)
        {
            return this.iProductCategory.UpdateActiveProductCategory(request);
        }
    }
}
