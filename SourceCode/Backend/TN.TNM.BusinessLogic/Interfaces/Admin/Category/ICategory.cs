using TN.TNM.BusinessLogic.Messages.Requests.Admin.Category;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Category;

namespace TN.TNM.BusinessLogic.Interfaces.Admin.Category
{
    public interface ICategory
    {
        /// <summary>
        /// Get all Category by CategoryTypeId
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        GetAllCategoryByCategoryTypeCodeResponse GetAllCategoryByCategoryTypeCode(
            GetAllCategoryByCategoryTypeCodeRequest request);

        /// <summary>
        /// Get all category
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetAllCategoryResponse GetAllCategory(GetAllCategoryRequest request);

        /// <summary>
        /// Get a category by id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetCategoryByIdResponse GetCategoryById(GetCategoryByIdRequest request);

        /// <summary>
        /// Create new Category
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        CreateCategoryResponse CreateCategory(CreateCategoryRequest request);

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        DeleteCategoryByIdResponse DeleteCategoryById(DeleteCategoryByIdRequest request);

        /// <summary>
        /// Edit a category
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        EditCategoryByIdResponse EditCategoryById(EditCategoryByIdRequest request);

        /// <summary>
        /// Update Status Is Active
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        UpdateStatusIsActiveResponse UpdateStatusIsActive(UpdateStatusIsActiveRequest request);

        /// <summary>
        /// Update Status Is Default
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        UpdateStatusIsDefaultResponse UpdateStatusIsDefault(UpdateStatusIsDefaultRequest request);
    }
}
