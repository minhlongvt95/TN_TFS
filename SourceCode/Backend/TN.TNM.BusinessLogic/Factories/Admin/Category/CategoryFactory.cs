using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Admin.Category;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Category;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Category;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Admin.Category
{
    public class CategoryFactory : BaseFactory, ICategory
    {
        private ICategoryDataAccess iCategoryDataAccess;

        public CategoryFactory(ICategoryDataAccess _iCategoryDataAccess, ILogger<CategoryFactory> _logger)
        {
            this.iCategoryDataAccess = _iCategoryDataAccess;
            this.logger = _logger;
        }

        public GetAllCategoryByCategoryTypeCodeResponse GetAllCategoryByCategoryTypeCode(
            GetAllCategoryByCategoryTypeCodeRequest request)
        {
            try
            {
                this.logger.LogInformation("Get all Category by categoryId");
                var parameter = request.ToParameter();
                var result = iCategoryDataAccess.GetAllCategoryByCategoryTypeCode(parameter);
                var response = new GetAllCategoryByCategoryTypeCodeResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    Category = new List<CategoryModel>(),
                    CategoryCVUList=new List<CategoryModel>(),
                    CategoryGENDERList=new List<CategoryModel>(),
                    CategoryLDOList=new List<CategoryModel>(),
                    CategoryLHIList=new List<CategoryModel>(),
                    CategoryLNGList=new List<CategoryModel>(),
                    CategoryNCHList=new List<CategoryModel>(),
                    CategoryNHAList=new List<CategoryModel>(),
                    CategoryPTOList=new List<CategoryModel>(),
                    CategoryQNGList=new List<CategoryModel>(),
                    CategoryTHAList=new List<CategoryModel>(),
                    CategoryTNHList=new List<CategoryModel>(),
                    CategoryDVIList = new List<CategoryModel>(),
                    CategoryPMList = new List<CategoryModel>(),
                    CategoryLabourContractList = new List<CategoryModel>(),
                    CategoryNCAList = new List<CategoryModel>()
                };
                result.Category.ForEach(categoryEntity =>
                {
                    response.Category.Add(new CategoryModel(categoryEntity));
                });
                result.CategoryCVUList.ForEach(categoryEntity =>
                {
                    response.CategoryCVUList.Add(new CategoryModel(categoryEntity));
                });
                result.CategoryGENDERList.ForEach(categoryEntity =>
                {
                    response.CategoryGENDERList.Add(new CategoryModel(categoryEntity));
                });
                result.CategoryLDOList.ForEach(categoryEntity =>
                {
                    response.CategoryLDOList.Add(new CategoryModel(categoryEntity));
                });
                result.CategoryLHIList.ForEach(categoryEntity =>
                {
                    response.CategoryLHIList.Add(new CategoryModel(categoryEntity));
                });
                result.CategoryLNGList.ForEach(categoryEntity =>
                {
                    response.CategoryLNGList.Add(new CategoryModel(categoryEntity));
                });
                result.CategoryNCHList.ForEach(categoryEntity =>
                {
                    response.CategoryNCHList.Add(new CategoryModel(categoryEntity));
                });
                result.CategoryNHAList.ForEach(categoryEntity =>
                {
                    response.CategoryNHAList.Add(new CategoryModel(categoryEntity));
                });
                result.CategoryPTOList.ForEach(categoryEntity =>
                {
                    response.CategoryPTOList.Add(new CategoryModel(categoryEntity));
                });
                result.CategoryQNGList.ForEach(categoryEntity =>
                {
                    response.CategoryQNGList.Add(new CategoryModel(categoryEntity));
                });
                result.CategoryTHAList.ForEach(categoryEntity =>
                {
                    response.CategoryTHAList.Add(new CategoryModel(categoryEntity));
                });
                result.CategoryTNHList.ForEach(categoryEntity =>
                {
                    response.CategoryTNHList.Add(new CategoryModel(categoryEntity));
                });
                result.CategoryPMList.ForEach(categoryEntity =>
                {
                    response.CategoryPMList.Add(new CategoryModel(categoryEntity));
                });
                result.CategoryDVIList.ForEach(categoryEntity =>
                {
                    response.CategoryDVIList.Add(new CategoryModel(categoryEntity));
                });
                result.CategoryLabourContractList.ForEach(categoryEntity =>
                {
                    response.CategoryLabourContractList.Add(new CategoryModel(categoryEntity));
                });
                result.CategoryNCAList.ForEach(categoryEntity =>
                {
                    response.CategoryNCAList.Add(new CategoryModel(categoryEntity));
                });
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetAllCategoryByCategoryTypeCodeResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public GetCategoryByIdResponse GetCategoryById(GetCategoryByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get all Category by categoryId");
                var parameter = request.ToParameter();
                var result = iCategoryDataAccess.GetCategoryById(parameter);
                var response = new GetCategoryByIdResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    Category = result.Status ? new CategoryModel(result.Category) : null,
                    IsCategory = result.Status ?result.IsCategory:false,
                    MessageCode = result.Status ? "" : result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetCategoryByIdResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public GetAllCategoryResponse GetAllCategory(GetAllCategoryRequest request)
        {
            try
            {
                this.logger.LogInformation("Get all Category");
                var parameter = request.ToParameter();
                var result = iCategoryDataAccess.GetAllCategory(parameter);
                var response = new GetAllCategoryResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    CategoryTypeList = new List<CategoryTypeModel>()
                };
                result.CategoryTypeList.ForEach(categoryEntity =>
                {
                    var cate = new CategoryTypeModel(categoryEntity);
                    var cList = new List<CategoryModel>();
                    categoryEntity.CategoryList.ForEach(child =>
                    {
                        cList.Add(new CategoryModel(child));
                    });
                    cate.CategoryList = cList;
                    response.CategoryTypeList.Add(cate);
                });
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetAllCategoryResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public CreateCategoryResponse CreateCategory(CreateCategoryRequest request)
        {
            try
            {
                this.logger.LogInformation("Create Category");
                var parameter = request.ToParameter();
                var result = iCategoryDataAccess.CreateCategory(parameter);
                return new CreateCategoryResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new CreateCategoryResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.MasterData.CREATE_FAIL
                };
            }
        }

        public DeleteCategoryByIdResponse DeleteCategoryById(DeleteCategoryByIdRequest request)
        {
            try
            {
                logger.LogInformation("Delete Category");
                var parameter = request.ToParameter();
                var result = iCategoryDataAccess.DeleteCategoryById(parameter);
                return new DeleteCategoryByIdResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new DeleteCategoryByIdResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.MasterData.DELETE_FAIL
                };
            }
        }

        public EditCategoryByIdResponse EditCategoryById(EditCategoryByIdRequest request)
        {
            try
            {
                logger.LogInformation("Edit Category by Id");
                var parameter = request.ToParameter();
                var result = iCategoryDataAccess.EditCategoryById(parameter);
                return new EditCategoryByIdResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new EditCategoryByIdResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.MasterData.EDIT_FAIL
                };
            }
        }

        public UpdateStatusIsActiveResponse UpdateStatusIsActive(UpdateStatusIsActiveRequest request)
        {
            try
            {
                logger.LogInformation("Edit Category by Id");
                var parameter = request.ToParameter();
                var result = iCategoryDataAccess.UpdateStatusIsActive(parameter);
                return new UpdateStatusIsActiveResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new UpdateStatusIsActiveResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.MasterData.EDIT_FAIL
                };
            }
        }

        public UpdateStatusIsDefaultResponse UpdateStatusIsDefault(UpdateStatusIsDefaultRequest request)
        {
            try
            {
                logger.LogInformation("Edit Category by Id");
                var parameter = request.ToParameter();
                var result = iCategoryDataAccess.UpdateStatusIsDefault(parameter);
                return new UpdateStatusIsDefaultResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new UpdateStatusIsDefaultResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.MasterData.EDIT_FAIL
                };
            }
        }
    }
}
