using TN.TNM.BusinessLogic.Messages.Requests.Admin.ProductCategory;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.ProductCategory;

namespace TN.TNM.BusinessLogic.Interfaces.Admin.ProductCategory
{
    public interface IProductCategory
    {
        CreateProductCategoryResponse CreateProductCategory(CreateProductCategoryRequest request);
        GetAllProductCategoryResponse GetAllProductCategory(GetAllProductCategoryRequest request);
        GetProductCategoryByIdResponse GetProductCategoryById(GetProductCategoryByIdRequest request);
        EditProductCategoryResponse EditProductCategory(EditProductCategoryRequest request);
        DeleteProductCategoryResponse DeleteProductCategory(DeleteProductCategoryRequest request);
        SearchProductCategoryResponse SearchProductCategory(SearchProductCategoryRequest request);
        GetNameTreeProductCategoryResponse GetNameTreeProductCategory(GetNameTreeProductCategoryRequest request);
        GetAllCategoryCodeResponse GetAllCategoryCode(GetAllCategoryCodeRequest request);
        UpdateActiveProductCategoryResponse UpdateActiveProductCategory(UpdateActiveProductCategoryRequest request);
    }
}
