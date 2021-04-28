using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.Category;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Category;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Category;

namespace TN.TNM.Api.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategory iCategory;
        public CategoryController(ICategory _iCategory)
        {
            this.iCategory = _iCategory;
        }

        /// <summary>
        /// Get category info by categoty type code
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category/getAllCategoryByCategoryTypeCode")]
        [Authorize(Policy = "Member")]
        public GetAllCategoryByCategoryTypeCodeResponse GetAllCategoryByCategoryTypeCode([FromBody]GetAllCategoryByCategoryTypeCodeRequest request)
        {
            return this.iCategory.GetAllCategoryByCategoryTypeCode(request);
        }

        /// <summary>
        /// Get category info by categoty type code
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category/getAllCategory")]
        [Authorize(Policy = "Member")]
        public GetAllCategoryResponse GetAllCategoryResponse([FromBody]GetAllCategoryRequest request)
        {
            return this.iCategory.GetAllCategory(request);
        }

        /// <summary>
        /// Get category by id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category/getCategoryById")]
        [Authorize(Policy = "Member")]
        public GetCategoryByIdResponse GetCategoryByIdResponse([FromBody]GetCategoryByIdRequest request)
        {
            return this.iCategory.GetCategoryById(request);
        }

        /// <summary>
        /// Create new category
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category/createCategory")]
        [Authorize(Policy = "Member")]
        public CreateCategoryResponse CreateCategory([FromBody]CreateCategoryRequest request)
        {
            return this.iCategory.CreateCategory(request);
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category/deleteCategoryById")]
        [Authorize(Policy = "Member")]
        public DeleteCategoryByIdResponse DeleteCategoryById([FromBody]DeleteCategoryByIdRequest request)
        {
            return this.iCategory.DeleteCategoryById(request);
        }

        /// <summary>
        /// Edit a category
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category/editCategoryById")]
        [Authorize(Policy = "Member")]
        public EditCategoryByIdResponse Ed([FromBody]EditCategoryByIdRequest request)
        {
            return this.iCategory.EditCategoryById(request);
        }

        /// <summary>
        /// Edit a category
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category/updateStatusIsActive")]
        [Authorize(Policy = "Member")]
        public UpdateStatusIsActiveResponse UpdateStatusIsActive([FromBody]UpdateStatusIsActiveRequest request)
        {
            return this.iCategory.UpdateStatusIsActive(request);
        }

        /// <summary>
        /// Edit a category
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category/updateStatusIsDefault")]
        [Authorize(Policy = "Member")]
        public UpdateStatusIsDefaultResponse UpdateStatusIsDefault([FromBody]UpdateStatusIsDefaultRequest request)
        {
            return this.iCategory.UpdateStatusIsDefault(request);
        }
    }
}