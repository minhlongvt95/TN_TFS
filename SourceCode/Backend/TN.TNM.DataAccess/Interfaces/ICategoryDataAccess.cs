using TN.TNM.DataAccess.Messages.Parameters.Admin.Category;
using TN.TNM.DataAccess.Messages.Results.Admin.Category;
using TN.TNM.DataAccess.Messages.Results.Admin.Company;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface ICategoryDataAccess
    {
        /// <summary>
        /// Get all Category by CategoryTypeId
        /// </summary>
        /// <param name="parameter">parameter</param>
        /// <returns></returns>
        GetAllCategoryByCategoryTypeCodeResult GetAllCategoryByCategoryTypeCode(GetAllCategoryByCategoryTypeCodeParameter parameter);

        /// <summary>
        /// Get all category
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        GetAllCategoryResult GetAllCategory(GetAllCategoryParameter parameter);

        /// <summary>
        /// Get a category by id
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        GetCategoryByIdResult GetCategoryById(GetCategoryByIdParameter parameter);

        /// <summary>
        /// Create new category
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        CreateCategoryResult CreateCategory(CreateCategoryParameter parameter);

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        DeleteCategoryByIdResult DeleteCategoryById(DeleteCategoryByIdParameter parameter);

        /// <summary>
        /// Edit a category
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        EditCategoryByIdResult EditCategoryById(EditCategoryByIdParameter parameter);

        /// <summary>
        /// Update Status Is Active
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        UpdateStatusIsActiveResult UpdateStatusIsActive(UpdateStatusIsActiveParameter parameter);

        /// <summary>
        /// Update Status Is Default
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        UpdateStatusIsDefaultResult UpdateStatusIsDefault(UpdateStatusIsDefaultParameter parameter);
    }
}
