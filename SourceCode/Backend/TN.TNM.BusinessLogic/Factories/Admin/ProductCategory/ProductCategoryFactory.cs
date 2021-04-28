using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Admin.ProductCategory;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.ProductCategory;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.ProductCategory;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Admin.ProductCategory
{
    public class ProductCategoryFactory: BaseFactory, IProductCategory
    {
        private IProductCategoryDataAccess iProductCategoryDataAccess;
        public ProductCategoryFactory(IProductCategoryDataAccess _iProductCategoryDataAccess, ILogger<ProductCategoryFactory> _logger)
        {
            this.iProductCategoryDataAccess = _iProductCategoryDataAccess;
            this.logger = _logger;
        }

        public CreateProductCategoryResponse CreateProductCategory(CreateProductCategoryRequest request)
        {
            try
            {
                this.logger.LogInformation("Create Product Category");
                var parameter = request.ToParameter();
                var result = iProductCategoryDataAccess.CreateProductCategory(parameter);
                var response = new CreateProductCategoryResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    ProductCategoryId = result.ProductCategoryId
                };
                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogInformation(ex.Message);
                var response = new CreateProductCategoryResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
                return response;
            }
        }
        public GetAllProductCategoryResponse GetAllProductCategory(GetAllProductCategoryRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All Product Category");
                var parameter = request.ToParameter();
                var result = iProductCategoryDataAccess.GetAllProductCategory(parameter);
                var response = new GetAllProductCategoryResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ProductCategoryList = new List<Models.Admin.ProductCategoryModel>()
                };
                result.ProductCategoryList.ForEach(prcategory => {
                    response.ProductCategoryList.Add(new Models.Admin.ProductCategoryModel(prcategory));
                });
                response.MessageCode = result.Message;
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetAllProductCategoryResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }
        public GetProductCategoryByIdResponse GetProductCategoryById(GetProductCategoryByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Product Category by Id");
                var parameter = request.ToParameter();
                var result = iProductCategoryDataAccess.GetProductCategoryById(parameter);
                var response = new GetProductCategoryByIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ProductCategory = new Models.Admin.ProductCategoryModel(result.ProductCategory),
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetProductCategoryByIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }
        public EditProductCategoryResponse EditProductCategory(EditProductCategoryRequest request)
        {
            try
            {
                this.logger.LogInformation("Edit Product Category");
                var parameter = request.ToParameter();
                var result = iProductCategoryDataAccess.EditProductCategory(parameter);
                var response = new EditProductCategoryResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new EditProductCategoryResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
        public DeleteProductCategoryResponse DeleteProductCategory(DeleteProductCategoryRequest request)
        {
            try {
                this.logger.LogInformation("Delete Product Category");
                var parameter = request.ToParameter();
                var result = iProductCategoryDataAccess.DeleteProductCategory(parameter);
                var response = new DeleteProductCategoryResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;
            } catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new DeleteProductCategoryResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
        public SearchProductCategoryResponse SearchProductCategory(SearchProductCategoryRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Product Category");
                var parameter = request.ToParameter();
                var result = iProductCategoryDataAccess.SearchProductCategory(parameter);
                var response = new SearchProductCategoryResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    ProductCategoryList = new List<Models.Admin.ProductCategoryModel> ()
                };
                result.ProductCategoryList.ForEach(prc => response.ProductCategoryList.Add(new Models.Admin.ProductCategoryModel(prc)));
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new SearchProductCategoryResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.message.exception"
                };
            }
        }

        public GetNameTreeProductCategoryResponse GetNameTreeProductCategory(GetNameTreeProductCategoryRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Product Category");
                var parameter = request.ToParameter();
                var result = iProductCategoryDataAccess.GetNameTreeProductCategory(parameter);
                var response = new GetNameTreeProductCategoryResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    NameTree=result.NameTree
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetNameTreeProductCategoryResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.message.exception"
                };
            }
        }

        public GetAllCategoryCodeResponse GetAllCategoryCode(GetAllCategoryCodeRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All Category Code");
                var parameter = request.ToParameter();
                var result = iProductCategoryDataAccess.GetAllCategoryCode(parameter);

                var response = new GetAllCategoryCodeResponse
                {
                    MessageCode=result.Message,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ProductCategoryCodeList = result.ProductCategoryCodeList
                };
                return response;
            }
            catch (Exception ex)
            {
                return new GetAllCategoryCodeResponse
                {
                    MessageCode = ex.ToString(),
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public UpdateActiveProductCategoryResponse UpdateActiveProductCategory(UpdateActiveProductCategoryRequest request)
        {
            try
            {
                this.logger.LogInformation("Update Active Product Category");
                var parameter = request.ToParameter();
                var result = iProductCategoryDataAccess.UpdateActiveProductCategory(parameter);

                var response = new UpdateActiveProductCategoryResponse
                {
                    MessageCode = result.Message,
                    StatusCode = System.Net.HttpStatusCode.OK,
                };
                return response;
            }
            catch (Exception ex)
            {
                return new UpdateActiveProductCategoryResponse
                {
                    MessageCode = ex.ToString(),
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }
    }
}

