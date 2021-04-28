using System;
using System.Collections.Generic;
using System.Linq;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.ProductCategory;
using TN.TNM.DataAccess.Messages.Results.Admin.ProductCategory;
using TN.TNM.DataAccess.Models.ProductCategory;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class ProductCategoryDAO : BaseDAO, IProductCategoryDataAccess
    {
        public ProductCategoryDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetAllProductCategoryResult GetAllProductCategory(GetAllProductCategoryParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETALL, ObjectName.PRODUCTCATEGORY, "GetAllProductCategory", parameter.UserId);
            var products = context.Product.ToList();
            var productCategories = context.ProductCategory.ToList();
            var list = context.ProductCategory.Where(p=>p.Active == true).Select(o => new ProductCategoryEntityModel()
            {
                ProductCategoryId = o.ProductCategoryId,
                ProductCategoryName = o.ProductCategoryName,
                ProductCategoryLevel = o.ProductCategoryLevel,
                ProductCategoryCode = o.ProductCategoryCode,
                ParentId = o.ParentId,
                Description = o.Description,
                CreatedById = o.CreatedById,
                CreatedDate = o.CreatedDate,
                UpdatedById = o.UpdatedById,
                UpdatedDate = o.UpdatedDate,
                Active = o.Active.Value,
                CountProductCategory = CountProductCategory(o.ProductCategoryId, products, productCategories)
            }).ToList();


            //list.ForEach(item => {
            //    if (item.ParentId == null)
            //    {
            //        item.ProductCategoryChildList = GetChildren(item.ProductCategoryId, list);
            //    }
            //});

            list.ForEach(item =>
            {                       
                item.ProductCategoryChildList = GetChildren(item.ProductCategoryId, list);             
            });

            list = list.OrderBy(w => w.ProductCategoryLevel).ToList();

            return new GetAllProductCategoryResult
            {
                ProductCategoryList = list
            };
        }

        public CreateProductCategoryResult CreateProductCategory(CreateProductCategoryParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.ADD, ObjectName.PRODUCTCATEGORY, "Create Product Category", parameter.UserId);
                ProductCategory productCategory = new ProductCategory
                {
                    //ProductCategoryId = Guid.NewGuid(),
                    ProductCategoryName = parameter.ProductCategoryName,
                    ProductCategoryLevel = parameter.ProducCategoryLevel,
                    ProductCategoryCode = parameter.ProductCategoryCode,
                    Description = parameter.Description,
                    ParentId = parameter.ParentId,
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now,
                    Active = true,
                };
                context.ProductCategory.Add(productCategory);
                context.SaveChanges();
                return new CreateProductCategoryResult
                {
                    Status = true,
                    Message = CommonMessage.ProductCategory.CREATE_SUCCESS,
                    ProductCategoryId = productCategory.ProductCategoryId
                };
            }
            catch (Exception e)
            {
                return new CreateProductCategoryResult
                {
                    Status = false,
                    Message = e.ToString()
                };
            }
          
        }

        public EditProductCategoryResult EditProductCategory(EditProductCategoryParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.UPDATE, ObjectName.PRODUCTCATEGORY, "Edit Product Category", parameter.UserId);
            var productcategory = context.ProductCategory.FirstOrDefault(pc => pc.ProductCategoryId == parameter.ProductCategoryId);
            var checkduplicate = context.ProductCategory.FirstOrDefault(pc => pc.ProductCategoryCode == parameter.ProductCategoryCode && pc.ProductCategoryId != parameter.ProductCategoryId);
            if (checkduplicate != null)
            {
                return new EditProductCategoryResult
                {
                    Status = false,
                    Message = CommonMessage.Organization.DUPLICATE_CODE
                };
            }

            if (productcategory != null)
            {
                productcategory.ProductCategoryName = parameter.ProductCategoryName;
                productcategory.ProductCategoryCode = parameter.ProductCategoryCode;
                productcategory.Description = parameter.Description;
                //productcategory.Active = parameter.Active;
                productcategory.UpdatedById = parameter.UserId;
                productcategory.UpdatedDate = DateTime.Now;

                context.SaveChanges();

                return new EditProductCategoryResult
                {
                    Status = true,
                    Message = CommonMessage.ProductCategory.EDIT_SUCCESS
                };
            } else
            {
                return new EditProductCategoryResult
                {
                    Status = false,
                    Message = CommonMessage.ProductCategory.EDIT_FAIL
                };
            }
        }

        public GetProductCategoryByIdResult GetProductCategoryById(GetProductCategoryByIdParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETBYID, ObjectName.PRODUCTCATEGORY, "Get Product Category by ID", parameter.UserId);
            var prc = context.ProductCategory.FirstOrDefault(pc => pc.ProductCategoryId == parameter.ProductCategoryId);
            if (prc != null)
            {
                ProductCategoryEntityModel productcategory = new ProductCategoryEntityModel
                {
                    ProductCategoryId = prc.ProductCategoryId,
                    ProductCategoryName = prc.ProductCategoryName,
                    ProductCategoryCode = prc.ProductCategoryCode,
                    ProductCategoryLevel = prc.ProductCategoryLevel,
                    ParentId=prc.ParentId,
                    Description = prc.Description,
                    CreatedById = prc.CreatedById,
                    CreatedDate = prc.CreatedDate,
                    UpdatedById = prc.UpdatedById,
                    UpdatedDate = prc.UpdatedDate,
                    Active = prc.Active.Value
                };
                return new GetProductCategoryByIdResult
                {
                    Status = true,
                    ProductCategory = productcategory
                };
            }
            else
            {
                return new GetProductCategoryByIdResult
                {
                    Status =false,
                    ProductCategory = null,
                    Message = CommonMessage.ProductCategory.GET_FAIL
                };
            }
        }

        public DeleteProductCategoryResult DeleteProductCategory(DeleteProductCategoryParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.DELETE, ObjectName.PRODUCTCATEGORY, "Delete ProductCategoryById", parameter.UserId);
            var productcategorychild = context.ProductCategory.Where(pc => pc.ParentId == parameter.ProductCategoryId).ToList();
            if (productcategorychild.Count > 0)
            {
                return new DeleteProductCategoryResult
                {
                    Status = false,
                    Message = CommonMessage.ProductCategory.HAS_CHILD
                };
            }
            var productcategory = context.ProductCategory.FirstOrDefault(pc => pc.ProductCategoryId == parameter.ProductCategoryId);
            if (productcategory != null)
            {
                context.ProductCategory.Remove(productcategory);
                context.SaveChanges();
                return new DeleteProductCategoryResult
                {
                    Status = true,
                    Message = CommonMessage.ProductCategory.DELETE_SUCCESS
                };
            }
            else
            {
                return new DeleteProductCategoryResult {
                    Status = false,
                    Message = CommonMessage.ProductCategory.DELETE_FAIL
                };
            }
        }

        public SearchProductCategoryResult SearchProductCategory(SearchProductCategoryParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.SEARCH, ObjectName.PRODUCTCATEGORY, "Search ProductCategory", parameter.UserId);
            var products = context.Product.ToList();
            var productCategories = context.ProductCategory.ToList();
            var productcategorylist = context.ProductCategory.Where(prc => (prc.ProductCategoryName.Contains(parameter.Keyword_name) || parameter.Keyword_name == null || parameter.Keyword_name == "") &&
                                                                           (prc.ProductCategoryCode.Contains(parameter.Keyword_code) || parameter.Keyword_code == null || parameter.Keyword_code == "") &&
                                                                           (prc.Active == true))
            .Select(s => new ProductCategoryEntityModel
            {
                ProductCategoryId = s.ProductCategoryId,
                ProductCategoryCode = s.ProductCategoryCode,
                ProductCategoryName = s.ProductCategoryName,
                ProductCategoryLevel = s.ProductCategoryLevel,
                Description = s.Description,
                Active = s.Active.Value,
                CreatedById = s.CreatedById,
                ParentId = s.ParentId,
                CreatedDate = s.CreatedDate,
                UpdatedById = s.UpdatedById,
                UpdatedDate = s.UpdatedDate,
                ProductCategoryChildList = new List<ProductCategoryEntityModel>(),
                CountProductCategory = CountProductCategory(s.ProductCategoryId, products, productCategories)
            }).OrderByDescending(n => n.CreatedDate).ToList();
            return new SearchProductCategoryResult
            {
                Status = productcategorylist.Count > 0,
                Message = productcategorylist.Count > 0 ? "" : CommonMessage.ProductCategory.NO_PRODUCTCATEGORY,
                ProductCategoryList = productcategorylist.Count > 0 ? productcategorylist : null
            };
        }

        private List<ProductCategoryEntityModel> GetChildren(Guid? id, List<ProductCategoryEntityModel> list)
        {
            return list.Where(l => l.ParentId == id)
                .Select(l => new ProductCategoryEntityModel()
                {
                    ProductCategoryId = l.ProductCategoryId,
                    ProductCategoryName = l.ProductCategoryName,
                    ProductCategoryCode = l.ProductCategoryCode,
                    ProductCategoryLevel = l.ProductCategoryLevel,
                    Description = l.Description,
                    CreatedById = l.CreatedById,
                    CreatedDate = l.CreatedDate,
                    UpdatedById = l.UpdatedById,
                    UpdatedDate = l.UpdatedDate,
                    Active = l.Active,
                    ParentId = l.ParentId,
                    ProductCategoryChildList = GetChildren(l.ProductCategoryId, list)
                }).OrderBy(l => l.ProductCategoryName).ToList();
        }

        public GetNameTreeProductCategoryResult GetNameTreeProductCategory(GetNameTreeProductCategoryParameter parameter)
        {
            try
            {
                return new GetNameTreeProductCategoryResult
                {
                    NameTree = GetNameTree(parameter.ProductCategoryID),
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new GetNameTreeProductCategoryResult
                {
                    Message = ex.ToString(),
                    Status = false
                };
            }
        }

        private string GetNameTree(Guid? Id)
        {
            string result = string.Empty;
            var ProductCategory = context.ProductCategory.Where(m => m.ProductCategoryId == Id).FirstOrDefault();
            if (ProductCategory.ProductCategoryLevel == 0)
            {
                return ProductCategory.ProductCategoryName;
            }
            else
            {
                result= GetNameTree(ProductCategory.ParentId)+">"+ ProductCategory.ProductCategoryName;
            }
            return result;
        }

        public GetAllCategoryCodeResult GetAllCategoryCode(GetAllCategoryCodeParameter parameter)
        {
            try
            {
                var listCode = context.ProductCategory.Select(item => new {code=item.ProductCategoryCode.ToString().ToLower()}).ToList();
                List<string> result = new List<string>();
                foreach (var item in listCode)
                {
                    result.Add(item.code);
                }

                return new GetAllCategoryCodeResult
                {
                    Message = "Sucesss",
                    Status = true,
                    ProductCategoryCodeList= result
                };
            }
            catch (Exception ex)
            {
                return new GetAllCategoryCodeResult
                {
                    Message = ex.ToString(),
                    Status = false
                };
            }
        }

        public int CountProductCategory(Guid productCategoryId, List<Product> products, List<ProductCategory> productCategories)
        {
            var count = products.Where(p=>p.ProductCategoryId == productCategoryId).Count();
            count += productCategories.Where(pc => pc.ParentId == productCategoryId).Count();
            return count;
        }
        
        public UpdateActiveProductCategoryResult UpdateActiveProductCategory(UpdateActiveProductCategoryParameter parameter)
        {
            try
            {
                var productCategoryById = context.ProductCategory.FirstOrDefault(item => item.ProductCategoryId == parameter.ProductCategoryId);
                productCategoryById.Active = false;
                productCategoryById.CreatedById = parameter.UserId;
                productCategoryById.CreatedDate = DateTime.Now;

                context.ProductCategory.Update(productCategoryById);
                context.SaveChanges();

                return new UpdateActiveProductCategoryResult
                {
                    Message = CommonMessage.ProductCategory.DELETE_SUCCESS,
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new UpdateActiveProductCategoryResult
                {
                    Message = CommonMessage.ProductCategory.DELETE_FAIL,
                    Status = false
                };
            }
        }
    }
}
