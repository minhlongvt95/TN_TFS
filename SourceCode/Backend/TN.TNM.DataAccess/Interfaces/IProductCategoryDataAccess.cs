using TN.TNM.DataAccess.Messages.Parameters.Admin.ProductCategory;
using TN.TNM.DataAccess.Messages.Results.Admin.ProductCategory;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IProductCategoryDataAccess
    {
        GetAllProductCategoryResult GetAllProductCategory(GetAllProductCategoryParameter parameter);
        CreateProductCategoryResult CreateProductCategory(CreateProductCategoryParameter parameter);
        GetProductCategoryByIdResult GetProductCategoryById(GetProductCategoryByIdParameter parameter);
        EditProductCategoryResult EditProductCategory(EditProductCategoryParameter parameter);
        DeleteProductCategoryResult DeleteProductCategory(DeleteProductCategoryParameter parameter);
        SearchProductCategoryResult SearchProductCategory(SearchProductCategoryParameter parameter);
        GetNameTreeProductCategoryResult GetNameTreeProductCategory(GetNameTreeProductCategoryParameter parameter);
        GetAllCategoryCodeResult GetAllCategoryCode(GetAllCategoryCodeParameter parameter);
        UpdateActiveProductCategoryResult UpdateActiveProductCategory(UpdateActiveProductCategoryParameter parameter);
    }
}
